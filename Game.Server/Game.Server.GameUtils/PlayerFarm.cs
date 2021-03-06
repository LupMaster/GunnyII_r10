using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class PlayerFarm : PlayerFarmInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected GamePlayer m_player;
		private bool m_saveToDb;
		private List<UserFieldInfo> m_removedList = new List<UserFieldInfo>();
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
		}
		public UserFarmInfo OtherFarm
		{
			get
			{
				return this.m_otherFarm;
			}
		}
		public UserFieldInfo[] OtherFields
		{
			get
			{
				return this.m_otherFields;
			}
		}
		public UserFarmInfo CurrentFarm
		{
			get
			{
				return this.m_farm;
			}
		}
		public UserFieldInfo[] CurrentFields
		{
			get
			{
				return this.m_fields;
			}
		}
		public PlayerFarm(GamePlayer player, bool saveTodb, int capibility, int beginSlot) : base(capibility, beginSlot)
		{
			this.m_player = player;
			this.m_saveToDb = saveTodb;
		}
		public virtual void LoadFromDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					UserFarmInfo singleFarm = playerBussiness.GetSingleFarm(this.m_player.PlayerCharacter.ID);
					UserFieldInfo[] singleFields = playerBussiness.GetSingleFields(this.m_player.PlayerCharacter.ID);
					this.UpdateFarm(singleFarm);
					UserFieldInfo[] array = singleFields;
					for (int i = 0; i < array.Length; i++)
					{
						UserFieldInfo userFieldInfo = array[i];
						this.AddFieldTo(userFieldInfo, userFieldInfo.FieldID, singleFarm.FarmID);
					}
				}
			}
		}
		public virtual void SaveToDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					object @lock;
					Monitor.Enter(@lock = this.m_lock);
					try
					{
						if (this.m_farm != null && this.m_farm.IsDirty)
						{
							if (this.m_farm.ID > 0)
							{
								playerBussiness.UpdateFarm(this.m_farm);
							}
							else
							{
								playerBussiness.AddFarm(this.m_farm);
							}
							for (int i = 0; i < this.m_fields.Length; i++)
							{
								UserFieldInfo userFieldInfo = this.m_fields[i];
								if (userFieldInfo != null && userFieldInfo.IsDirty)
								{
									if (userFieldInfo.ID > 0)
									{
										playerBussiness.UpdateFields(userFieldInfo);
									}
									else
									{
										playerBussiness.AddFields(userFieldInfo);
									}
								}
							}
						}
					}
					finally
					{
						Monitor.Exit(@lock);
					}
				}
			}
		}
		public bool AddFieldTo(UserFieldInfo item, int place, int farmId)
		{
			if (base.AddFieldTo(item, place))
			{
				item.FarmID = farmId;
				return true;
			}
			return false;
		}
		public bool AddOtherFieldTo(UserFieldInfo item, int place, int farmId)
		{
			if (base.AddOtherFieldTo(item, place))
			{
				item.FarmID = farmId;
				return true;
			}
			return false;
		}
		public void EnterFarm()
		{
			this.CropHelperSwitchField(false);
			if (this.m_farm == null)
			{
				this.CreateFarm(this.m_player.PlayerCharacter.ID, this.m_player.PlayerCharacter.NickName);
			}
			if (this.AccelerateTimeFields())
			{
				this.m_player.Out.SendEnterFarm(this.m_player.PlayerCharacter, this.CurrentFarm, this.GetFields());
				this.m_farmstatus = 1;
			}
		}
		public void CropHelperSwitchField(bool isStopFarmHelper)
		{
			if (this.m_farm != null && this.m_farm.isFarmHelper)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(this.m_farm.isAutoId);
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(itemTemplateInfo.Property4);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				int num = this.m_farm.AutoValidDate * 60;
				TimeSpan timeSpan = DateTime.Now - this.m_farm.AutoPayTime;
				int killCropId = this.m_farm.KillCropId;
				int num2;
				if (timeSpan.TotalMilliseconds < 0.0)
				{
					num2 = num;
				}
				else
				{
					num2 = num - (int)timeSpan.TotalMilliseconds;
				}
				int num3 = (1 - num2 / num) * killCropId / 1000;
				if (num3 > killCropId)
				{
					num3 = killCropId;
					isStopFarmHelper = true;
				}
				if (isStopFarmHelper)
				{
					itemInfo.Count = num3;
					if (num3 > 0)
					{
						string content = string.Concat(new object[]
						{
							"Kết thúc trợ thủ, bạn nhận được ",
							num3,
							" ",
							itemInfo.Template.Name
						});
						string title = "Kết thúc trợ thủ, nhận được thức ăn thú cưng!";
						this.m_player.SendItemToMail(itemInfo, content, title, eMailType.ItemOverdue);
						this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
					object @lock;
					Monitor.Enter(@lock = this.m_lock);
					try
					{
						this.m_farm.isFarmHelper = false;
						this.m_farm.isAutoId = 0;
						this.m_farm.AutoPayTime = DateTime.Now;
						this.m_farm.AutoValidDate = 0;
						this.m_farm.GainFieldId = 0;
						this.m_farm.KillCropId = 0;
					}
					finally
					{
						Monitor.Exit(@lock);
					}
					this.m_player.Out.SendHelperSwitchField(this.m_player.PlayerCharacter, this.m_farm);
				}
			}
		}
		public void ExitFarm()
		{
			this.m_farmstatus = 0;
		}
		public virtual void HelperSwitchField(bool isHelper, int seedID, int seedTime, int haveCount, int getCount)
		{
			if (base.HelperSwitchFields(isHelper, seedID, seedTime, haveCount, getCount))
			{
				this.m_player.Out.SendHelperSwitchField(this.m_player.PlayerCharacter, this.m_farm);
			}
		}
		public virtual bool GainFriendFields(int userId, int fieldId)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(userId);
			UserFieldInfo userFieldInfo = this.m_otherFields[fieldId];
			if (userFieldInfo == null)
			{
				return false;
			}
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(userFieldInfo.SeedID);
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(itemTemplateInfo.Property4);
			ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 102);
			List<ItemInfo> list = new List<ItemInfo>();
			this.AccelerateTimeFields();
			if (this.GetOtherFieldAt(fieldId).isDig())
			{
				return false;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				if (this.m_otherFields[fieldId].GainCount <= 9)
				{
					bool result = false;
					return result;
				}
				this.m_otherFields[fieldId].GainCount--;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			if (!this.m_player.PropBag.StackItemToAnother(item) && !this.m_player.PropBag.AddItem(item))
			{
				list.Add(item);
			}
			if (playerById == null)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					for (int i = 0; i < this.m_otherFields.Length; i++)
					{
						UserFieldInfo userFieldInfo2 = this.m_otherFields[i];
						if (userFieldInfo2 != null)
						{
							playerBussiness.UpdateFields(userFieldInfo2);
						}
					}
					goto IL_158;
				}
			}
			if (playerById.Farm.Status == 1)
			{
				playerById.Farm.UpdateGainCount(fieldId, this.m_otherFields[fieldId].GainCount);
				playerById.Out.SendtoGather(playerById.PlayerCharacter, this.m_otherFields[fieldId]);
			}
			IL_158:
			this.m_player.Out.SendtoGather(this.m_player.PlayerCharacter, this.m_otherFields[fieldId]);
			if (list.Count > 0)
			{
				this.m_player.SendItemsToMail(list, "Bagfull trả về thư!", "Bagfull trả về thư!", eMailType.ItemOverdue);
				this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			return true;
		}
		public void EnterFriendFarm(int userId)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(userId);
			UserFarmInfo userFarmInfo;
			UserFieldInfo[] array;
			if (playerById == null)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					userFarmInfo = playerBussiness.GetSingleFarm(userId);
					array = playerBussiness.GetSingleFields(userId);
					goto IL_5A;
				}
			}
			userFarmInfo = playerById.Farm.CurrentFarm;
			array = playerById.Farm.CurrentFields;
			playerById.ViFarmsAdd(this.m_player.PlayerCharacter.ID);
			IL_5A:
			if (userFarmInfo == null)
			{
				userFarmInfo = this.CreateFarmForNulll(userId);
				array = this.CreateFieldsForNull(userId);
			}
			this.m_farmstatus = this.m_player.PlayerCharacter.ID;
			this.UpdateOtherFarm(userFarmInfo);
			base.ClearOtherFields();
			UserFieldInfo[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				UserFieldInfo userFieldInfo = array2[i];
				if (userFieldInfo != null)
				{
					this.AddOtherFieldTo(userFieldInfo, userFieldInfo.FieldID, userFarmInfo.FarmID);
				}
			}
			if (this.AccelerateOtherTimeFields())
			{
				this.m_player.Out.SendEnterFarm(this.m_player.PlayerCharacter, this.OtherFarm, this.GetOtherFields());
			}
		}
		public virtual void PayField(List<int> fieldIds, int payFieldTime)
		{
			if (base.CreateField(this.m_player.PlayerCharacter.ID, fieldIds, payFieldTime))
			{
				foreach (int current in this.m_player.ViFarms)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(current);
					if (playerById != null && playerById.Farm.Status == current)
					{
						playerById.Out.SendPayFields(this.m_player, fieldIds);
					}
				}
				this.m_player.Out.SendPayFields(this.m_player, fieldIds);
			}
		}
		public override bool GrowField(int fieldId, int templateID)
		{
			if (base.GrowField(fieldId, templateID))
			{
				foreach (int current in this.m_player.ViFarms)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(current);
					if (playerById != null && playerById.Farm.Status == current)
					{
						playerById.Out.SendSeeding(this.m_player.PlayerCharacter, this.m_fields[fieldId]);
					}
				}
				this.m_player.Out.SendSeeding(this.m_player.PlayerCharacter, this.m_fields[fieldId]);
				return true;
			}
			return false;
		}
		public override bool killCropField(int fieldId)
		{
			if (base.killCropField(fieldId))
			{
				this.m_player.Out.SendKillCropField(this.m_player.PlayerCharacter, this.m_fields[fieldId]);
				return true;
			}
			return false;
		}
		public virtual bool GainField(int fieldId)
		{
			this.AccelerateTimeFields();
			if (this.GetFieldAt(fieldId).isDig())
			{
				return false;
			}
			if (fieldId < 0 || fieldId > this.GetFields().Count<UserFieldInfo>())
			{
				return false;
			}
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(this.m_fields[fieldId].SeedID);
			if (itemTemplateInfo == null)
			{
				return false;
			}
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(itemTemplateInfo.Property4);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
			List<ItemInfo> list = new List<ItemInfo>();
			itemInfo.Count = this.m_fields[fieldId].GainCount;
			if (base.killCropField(fieldId))
			{
				if (!this.m_player.PropBag.StackItemToAnother(itemInfo) && !this.m_player.PropBag.AddItem(itemInfo))
				{
					list.Add(itemInfo);
				}
				this.m_player.Out.SendtoGather(this.m_player.PlayerCharacter, this.m_fields[fieldId]);
				foreach (int current in this.m_player.ViFarms)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(current);
					if (playerById != null && playerById.Farm.Status == current)
					{
						playerById.Out.SendtoGather(this.m_player.PlayerCharacter, this.m_fields[fieldId]);
					}
				}
				this.m_player.OnCropPrimaryEvent();
				if (list.Count > 0)
				{
					this.m_player.SendItemsToMail(list, "Bagfull trả về thư!", "Bagfull trả về thư!", eMailType.ItemOverdue);
					this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				return true;
			}
			return false;
		}
	}
}
