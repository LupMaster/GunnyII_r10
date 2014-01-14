using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class PetInventory : PetAbstractInventory
	{
		protected GamePlayer m_player;
		private bool m_saveToDb;
		private List<UsersPetinfo> m_removedList;
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
		}
		public PetInventory(GamePlayer player, bool saveTodb, int capibility, int aCapability, int beginSlot) : base(capibility, aCapability, beginSlot)
		{
			this.m_player = player;
			this.m_saveToDb = saveTodb;
			this.m_removedList = new List<UsersPetinfo>();
		}
		public virtual void LoadFromDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					int iD = this.m_player.PlayerCharacter.ID;
					UsersPetinfo[] userPetSingles = playerBussiness.GetUserPetSingles(iD);
					UsersPetinfo[] userAdoptPetSingles = playerBussiness.GetUserAdoptPetSingles(iD);
					PetEquipDataInfo[] eqPetSingles = playerBussiness.GetEqPetSingles(iD);
					base.BeginChanges();
					try
					{
						UsersPetinfo[] array = userPetSingles;
						for (int i = 0; i < array.Length; i++)
						{
							UsersPetinfo usersPetinfo = array[i];
							usersPetinfo.EquipList = this.GetPetEquip(usersPetinfo.ID, eqPetSingles);
							this.AddPetTo(usersPetinfo, usersPetinfo.Place);
						}
						UsersPetinfo[] array2 = userAdoptPetSingles;
						for (int j = 0; j < array2.Length; j++)
						{
							UsersPetinfo usersPetinfo2 = array2[j];
							this.AddAdoptPetTo(usersPetinfo2, usersPetinfo2.Place);
						}
					}
					finally
					{
						base.CommitChanges();
					}
				}
			}
		}
		private Dictionary<int, PetEquipDataInfo> GetPetEquip(int petID, PetEquipDataInfo[] eqs)
		{
			Dictionary<int, PetEquipDataInfo> dictionary = new Dictionary<int, PetEquipDataInfo>();
			for (int i = 0; i < eqs.Length; i++)
			{
				PetEquipDataInfo petEquipDataInfo = eqs[i];
				if (petID == petEquipDataInfo.PetID)
				{
					dictionary.Add(petEquipDataInfo.eqType, petEquipDataInfo);
				}
			}
			return dictionary;
		}
		public virtual void SaveToDatabase(bool saveAdopt)
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					object @lock;
					Monitor.Enter(@lock = this.m_lock);
					try
					{
						for (int i = 0; i < this.m_pets.Length; i++)
						{
							UsersPetinfo usersPetinfo = this.m_pets[i];
							if (usersPetinfo != null && usersPetinfo.IsDirty)
							{
								if (usersPetinfo.ID > 0)
								{
									playerBussiness.UpdateUserPet(usersPetinfo);
								}
								else
								{
									playerBussiness.AddUserPet(usersPetinfo);
								}
								this.SaveqPet(playerBussiness, usersPetinfo);
							}
						}
						if (saveAdopt)
						{
							for (int j = 0; j < this.m_adoptPets.Length; j++)
							{
								UsersPetinfo usersPetinfo2 = this.m_adoptPets[j];
								if (usersPetinfo2 != null && usersPetinfo2.IsDirty && usersPetinfo2.ID == 0)
								{
									playerBussiness.AddUserAdoptPet(usersPetinfo2, false);
								}
							}
						}
					}
					finally
					{
						Monitor.Exit(@lock);
					}
					List<UsersPetinfo> removedList;
					Monitor.Enter(removedList = this.m_removedList);
					try
					{
						foreach (UsersPetinfo current in this.m_removedList)
						{
							playerBussiness.UpdateUserPet(current);
						}
						this.m_removedList.Clear();
					}
					finally
					{
						Monitor.Exit(removedList);
					}
				}
			}
		}
		public virtual void SaveqPet(PlayerBussiness pb, UsersPetinfo p)
		{
			for (int i = 0; i < p.EquipList.Count; i++)
			{
				PetEquipDataInfo petEquipDataInfo = p.EquipList[i];
				petEquipDataInfo.PetID = p.ID;
				if (petEquipDataInfo != null && petEquipDataInfo.IsDirty)
				{
					if (petEquipDataInfo.ID > 0)
					{
						pb.UpdateqPet(petEquipDataInfo);
					}
					else
					{
						pb.AddeqPet(petEquipDataInfo);
					}
				}
			}
		}
		public override bool AddPetTo(UsersPetinfo pet, int place)
		{
			if (pet.EquipList == null || pet.EquipList.Count == 0)
			{
				pet.EquipList = this.EmptyPetEquip(this.m_player.PlayerCharacter.ID);
			}
			if (base.AddPetTo(pet, place))
			{
				pet.UserID = this.m_player.PlayerCharacter.ID;
				return true;
			}
			return false;
		}
		private Dictionary<int, PetEquipDataInfo> EmptyPetEquip(int UserID)
		{
			Dictionary<int, PetEquipDataInfo> dictionary = new Dictionary<int, PetEquipDataInfo>();
			for (int i = 0; i < 3; i++)
			{
				dictionary.Add(i, new PetEquipDataInfo(null)
				{
					ID = 0,
					UserID = UserID,
					PetID = 0,
					eqType = i,
					eqTemplateID = 0,
					startTime = DateTime.Now,
					ValidDate = 0,
					IsExit = true
				});
			}
			return dictionary;
		}
		public override bool RemovePet(UsersPetinfo pet)
		{
			Dictionary<int, PetEquipDataInfo> equip = pet.GetEquip();
			if (base.RemovePet(pet))
			{
				this.MoveEqAllToBag(equip);
				List<UsersPetinfo> removedList;
				Monitor.Enter(removedList = this.m_removedList);
				try
				{
					pet.IsExit = false;
					this.m_removedList.Add(pet);
				}
				finally
				{
					Monitor.Exit(removedList);
				}
				return true;
			}
			return false;
		}
		public Dictionary<int, PetEquipDataInfo> RemoveEq(Dictionary<int, PetEquipDataInfo> Eqs)
		{
			Dictionary<int, PetEquipDataInfo> dictionary = new Dictionary<int, PetEquipDataInfo>();
			for (int i = 0; i < Eqs.Count; i++)
			{
				PetEquipDataInfo petEquipDataInfo = Eqs[i];
				petEquipDataInfo.eqTemplateID = 0;
				petEquipDataInfo.ValidDate = 0;
				dictionary.Add(i, petEquipDataInfo);
			}
			return dictionary;
		}
		public virtual bool MoveEqAllToBag(Dictionary<int, PetEquipDataInfo> eps)
		{
			int num = 0;
			for (int i = 0; i < eps.Count; i++)
			{
				this.MoveEqToBag(eps[i]);
				num++;
			}
			return num > 0;
		}
		public virtual void MoveEqToBag(PetEquipDataInfo ep)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(ep.eqTemplateID);
			if (itemTemplateInfo == null)
			{
				return;
			}
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
			itemInfo.IsUsed = true;
			itemInfo.IsBinds = true;
			itemInfo.ValidDate = ep.ValidDate;
			itemInfo.BeginDate = ep.startTime;
			List<ItemInfo> list = new List<ItemInfo>();
			if (!this.m_player.MainBag.AddItem(itemInfo))
			{
				list.Add(itemInfo);
			}
			if (list.Count > 0)
			{
				this.m_player.SendItemsToMail(list, "Bagfull trả về thư!", "Trả trang bị pet về thư!", eMailType.ItemOverdue);
				this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
		public virtual bool MoveEqFromBag(int place, int eqslot, ItemInfo item)
		{
			UsersPetinfo petAt = this.GetPetAt(place);
			if (petAt == null)
			{
				return false;
			}
			if (item.Template.Property2 > petAt.Level)
			{
				return false;
			}
			PetEquipDataInfo petEquipDataInfo = petAt.EquipList[eqslot];
			if (petEquipDataInfo.eqTemplateID > 0)
			{
				this.MoveEqToBag(petEquipDataInfo);
			}
			petEquipDataInfo.eqTemplateID = item.TemplateID;
			petEquipDataInfo.ValidDate = item.ValidDate;
			petEquipDataInfo.startTime = item.BeginDate;
			petEquipDataInfo = petEquipDataInfo.addTempalte(ItemMgr.FindItemTemplate(item.TemplateID));
			return this.UpdateQPet(place, eqslot, petEquipDataInfo);
		}
		public override bool AddAdoptPetTo(UsersPetinfo pet, int place)
		{
			return base.AddAdoptPetTo(pet, place);
		}
		public override bool RemoveAdoptPet(UsersPetinfo pet)
		{
			return base.RemoveAdoptPet(pet);
		}
		public override void UpdateChangedPlaces()
		{
			int[] slots = this.m_changedPlaces.ToArray();
			this.m_player.Out.SendUpdateUserPet(this, slots);
			base.UpdateChangedPlaces();
		}
		public virtual void ClearAdoptPets()
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				object @lock;
				Monitor.Enter(@lock = this.m_lock);
				try
				{
					for (int i = 0; i < base.ACapalility; i++)
					{
						if (this.m_adoptPets[i] != null && this.m_adoptPets[i].ID > 0)
						{
							playerBussiness.ClearAdoptPet(this.m_adoptPets[i].ID);
						}
						this.m_adoptPets[i] = null;
					}
				}
				finally
				{
					Monitor.Exit(@lock);
				}
			}
		}
	}
}
