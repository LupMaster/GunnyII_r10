using System;
namespace SqlDataProvider.Data
{
	public class ItemInfo : DataObject
	{
		private ItemTemplateInfo _template;
		private int _itemID;
		private int _userID;
		private int _bagType;
		private int _templateId;
		private int _place;
		private int _count;
		private bool _isJudage;
		private string _color;
		private bool _isExist;
		private int _strengthenLevel;
		private int _strengthenExp;
		private int _attackCompose;
		private int _defendCompose;
		private int _luckCompose;
		private int _agilityCompose;
		private bool _isBinds;
		private bool _isUsed;
		private string _skin;
		private DateTime _beginDate;
		private int _validDate;
		private DateTime _removeDate;
		private int _removeType;
		private int _hole1;
		private int _hole2;
		private int _hole3;
		private int _hole4;
		private int _hole5;
		private int _hole6;
		private int _strengthenTimes;
		private int _hole5Level;
		private int _hole6Level;
		private int _hole5Exp;
		private int _hole6Exp;
		private bool _isGold;
		private int _goldValidDate;
		private DateTime _goldBeginTime;
		private string _latentEnergyCurStr;
		private string _latentEnergyNewStr;
		private DateTime _latentEnergyEndTime;
		private string _pic;
		private bool _isTips;
		private bool _isLogs;
		private int _beadExp;
		private int _beadLevel;
		private bool _beadIsLock;
		private bool _isShowBind;
		private int _Damage;
		private int _Guard;
		private int _Blood;
		private int _Bless;
		public ItemTemplateInfo Template
		{
			get
			{
				return this._template;
			}
		}
		public int ItemID
		{
			get
			{
				return this._itemID;
			}
			set
			{
				this._itemID = value;
				this._isDirty = true;
			}
		}
		public int UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				this._userID = value;
				this._isDirty = true;
			}
		}
		public int BagType
		{
			get
			{
				return this._bagType;
			}
			set
			{
				this._bagType = value;
				this._isDirty = true;
			}
		}
		public int TemplateID
		{
			get
			{
				return this._templateId;
			}
			set
			{
				this._templateId = value;
				this._isDirty = true;
			}
		}
		public int Place
		{
			get
			{
				return this._place;
			}
			set
			{
				this._place = value;
				this._isDirty = true;
			}
		}
		public int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				this._count = value;
				this._isDirty = true;
			}
		}
		public bool IsJudge
		{
			get
			{
				return this._isJudage;
			}
			set
			{
				this._isJudage = value;
				this._isDirty = true;
			}
		}
		public string Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
				this._isDirty = true;
			}
		}
		public bool IsExist
		{
			get
			{
				return this._isExist;
			}
			set
			{
				this._isExist = value;
				this._isDirty = true;
			}
		}
		public int StrengthenLevel
		{
			get
			{
				return this._strengthenLevel;
			}
			set
			{
				this._strengthenLevel = value;
				this._isDirty = true;
			}
		}
		public int StrengthenExp
		{
			get
			{
				return this._strengthenExp;
			}
			set
			{
				this._strengthenExp = value;
				this._isDirty = true;
			}
		}
		public int AttackCompose
		{
			get
			{
				return this._attackCompose;
			}
			set
			{
				this._attackCompose = value;
				this._isDirty = true;
			}
		}
		public int DefendCompose
		{
			get
			{
				return this._defendCompose;
			}
			set
			{
				this._defendCompose = value;
				this._isDirty = true;
			}
		}
		public int LuckCompose
		{
			get
			{
				return this._luckCompose;
			}
			set
			{
				this._luckCompose = value;
				this._isDirty = true;
			}
		}
		public int AgilityCompose
		{
			get
			{
				return this._agilityCompose;
			}
			set
			{
				this._agilityCompose = value;
				this._isDirty = true;
			}
		}
		public bool IsBinds
		{
			get
			{
				return this._isBinds;
			}
			set
			{
				this._isBinds = value;
				this._isDirty = true;
			}
		}
		public bool IsUsed
		{
			get
			{
				return this._isUsed;
			}
			set
			{
				if (this._isUsed != value)
				{
					this._isUsed = value;
					this._isDirty = true;
				}
			}
		}
		public string Skin
		{
			get
			{
				return this._skin;
			}
			set
			{
				this._skin = value;
				this._isDirty = true;
			}
		}
		public DateTime BeginDate
		{
			get
			{
				return this._beginDate;
			}
			set
			{
				this._beginDate = value;
				this._isDirty = true;
			}
		}
		public int ValidDate
		{
			get
			{
				return this._validDate;
			}
			set
			{
				this._validDate = ((value > 999) ? 365 : value);
				this._isDirty = true;
			}
		}
		public DateTime RemoveDate
		{
			get
			{
				return this._removeDate;
			}
			set
			{
				this._removeDate = value;
				this._isDirty = true;
			}
		}
		public int RemoveType
		{
			get
			{
				return this._removeType;
			}
			set
			{
				this._removeType = value;
				this._removeDate = DateTime.Now;
				this._isDirty = true;
			}
		}
		public int Hole1
		{
			get
			{
				return this._hole1;
			}
			set
			{
				this._hole1 = value;
				this._isDirty = true;
			}
		}
		public int Hole2
		{
			get
			{
				return this._hole2;
			}
			set
			{
				this._hole2 = value;
				this._isDirty = true;
			}
		}
		public int Hole3
		{
			get
			{
				return this._hole3;
			}
			set
			{
				this._hole3 = value;
				this._isDirty = true;
			}
		}
		public int Hole4
		{
			get
			{
				return this._hole4;
			}
			set
			{
				this._hole4 = value;
				this._isDirty = true;
			}
		}
		public int Hole5
		{
			get
			{
				return this._hole5;
			}
			set
			{
				this._hole5 = value;
				this._isDirty = true;
			}
		}
		public int Hole6
		{
			get
			{
				return this._hole6;
			}
			set
			{
				this._hole6 = value;
				this._isDirty = true;
			}
		}
		public int StrengthenTimes
		{
			get
			{
				return this._strengthenTimes;
			}
			set
			{
				this._strengthenTimes = value;
				this._isDirty = true;
			}
		}
		public int Hole5Level
		{
			get
			{
				return this._hole5Level;
			}
			set
			{
				this._hole5Level = value;
				this._isDirty = true;
			}
		}
		public int Hole6Level
		{
			get
			{
				return this._hole6Level;
			}
			set
			{
				this._hole6Level = value;
				this._isDirty = true;
			}
		}
		public int Hole5Exp
		{
			get
			{
				return this._hole5Exp;
			}
			set
			{
				this._hole5Exp = value;
				this._isDirty = true;
			}
		}
		public int Hole6Exp
		{
			get
			{
				return this._hole6Exp;
			}
			set
			{
				this._hole6Exp = value;
				this._isDirty = true;
			}
		}
		public bool IsGold
		{
			get
			{
				return this._isGold;
			}
			set
			{
				this._isGold = value;
				this._isDirty = true;
			}
		}
		public int goldValidDate
		{
			get
			{
				return this._goldValidDate;
			}
			set
			{
				this._goldValidDate = value;
				this._isDirty = true;
			}
		}
		public DateTime goldBeginTime
		{
			get
			{
				return this._goldBeginTime;
			}
			set
			{
				this._goldBeginTime = value;
				this._isDirty = true;
			}
		}
		public string latentEnergyCurStr
		{
			get
			{
				return this._latentEnergyCurStr;
			}
			set
			{
				this._latentEnergyCurStr = value;
				this._isDirty = true;
			}
		}
		public string latentEnergyNewStr
		{
			get
			{
				return this._latentEnergyNewStr;
			}
			set
			{
				this._latentEnergyNewStr = value;
				this._isDirty = true;
			}
		}
		public DateTime latentEnergyEndTime
		{
			get
			{
				return this._latentEnergyEndTime;
			}
			set
			{
				this._latentEnergyEndTime = value;
				this._isDirty = true;
			}
		}
		public int Attack
		{
			get
			{
				return this._attackCompose + this._template.Attack;
			}
		}
		public string Pic
		{
			get
			{
				if (this._pic != null)
				{
					return this._pic;
				}
				return this._template.Pic;
			}
			set
			{
				this._pic = value;
			}
		}
		public int Defence
		{
			get
			{
				return this._defendCompose + this._template.Defence;
			}
		}
		public int Agility
		{
			get
			{
				return this._agilityCompose + this._template.Agility;
			}
		}
		public int Luck
		{
			get
			{
				return this._luckCompose + this._template.Luck;
			}
		}
		public bool IsTips
		{
			get
			{
				return this._isTips;
			}
			set
			{
				this._isTips = value;
			}
		}
		public bool IsLogs
		{
			get
			{
				return this._isLogs;
			}
			set
			{
				this._isLogs = value;
			}
		}
		public int beadExp
		{
			get
			{
				return this._beadExp;
			}
			set
			{
				this._beadExp = value;
				this._isDirty = true;
			}
		}
		public int beadLevel
		{
			get
			{
				return this._beadLevel;
			}
			set
			{
				this._beadLevel = value;
				this._isDirty = true;
			}
		}
		public bool beadIsLock
		{
			get
			{
				return this._beadIsLock;
			}
			set
			{
				this._beadIsLock = value;
				this._isDirty = true;
			}
		}
		public bool isShowBind
		{
			get
			{
				return this._isShowBind;
			}
			set
			{
				this._isShowBind = value;
				this._isDirty = true;
			}
		}
		public int Damage
		{
			get
			{
				return this._Damage;
			}
			set
			{
				this._Damage = value;
				this._isDirty = true;
			}
		}
		public int Guard
		{
			get
			{
				return this._Guard;
			}
			set
			{
				this._Guard = value;
				this._isDirty = true;
			}
		}
		public int Blood
		{
			get
			{
				return this._Blood;
			}
			set
			{
				this._Blood = value;
				this._isDirty = true;
			}
		}
		public int Bless
		{
			get
			{
				return this._Bless;
			}
			set
			{
				this._Bless = value;
				this._isDirty = true;
			}
		}
		public int GetBagType
		{
			get
			{
				return (int)this._template.BagType;
			}
		}
		public ItemInfo(ItemTemplateInfo temp)
		{
			this._template = temp;
		}
		public ItemInfo Clone()
		{
			return new ItemInfo(this._template)
			{
				_userID = this._userID,
				_validDate = this._validDate,
				_templateId = this._templateId,
				_strengthenLevel = this._strengthenLevel,
				_strengthenExp = this._strengthenExp,
				_luckCompose = this._luckCompose,
				_itemID = 0,
				_isJudage = this._isJudage,
				_isExist = this._isExist,
				_isBinds = this._isBinds,
				_isUsed = this._isUsed,
				_defendCompose = this._defendCompose,
				_count = this._count,
				_color = this._color,
				Skin = this._skin,
				_beginDate = this._beginDate,
				_attackCompose = this._attackCompose,
				_agilityCompose = this._agilityCompose,
				_bagType = this._bagType,
				_isDirty = true,
				_removeDate = this._removeDate,
				_removeType = this._removeType,
				_hole1 = this._hole1,
				_hole2 = this._hole2,
				_hole3 = this._hole3,
				_hole4 = this._hole4,
				_hole5 = this._hole5,
				_hole6 = this._hole6,
				_hole5Exp = this._hole5Exp,
				_hole5Level = this._hole5Level,
				_hole6Exp = this._hole6Exp,
				_hole6Level = this._hole6Level,
				_isGold = this._isGold,
				_goldBeginTime = this._goldBeginTime,
				_goldValidDate = this._goldValidDate,
				//_strengthenExp = this._strengthenExp,
				_latentEnergyCurStr = this._latentEnergyCurStr,
				_latentEnergyNewStr = this._latentEnergyNewStr,
				_latentEnergyEndTime = this._latentEnergyEndTime,
				_beadExp = this._beadExp,
				_beadLevel = this._beadLevel,
				_beadIsLock = this._beadIsLock,
				_isShowBind = this._isShowBind,
				_Damage = this._Damage,
				_Guard = this._Guard,
				_Bless = this._Bless,
				_Blood = this._Blood
			};
		}
		public void Copy(ItemInfo item)
		{
			this._userID = item.UserID;
			this._validDate = item.ValidDate;
			this._templateId = item.TemplateID;
			this._strengthenLevel = item.StrengthenLevel;
			this._strengthenExp = item.StrengthenExp;
			this._luckCompose = item.LuckCompose;
			this._itemID = 0;
			this._isJudage = item.IsJudge;
			this._isExist = item.IsExist;
			this._isBinds = item.IsBinds;
			this._isUsed = item.IsUsed;
			this._defendCompose = item.DefendCompose;
			this._count = item.Count;
			this._color = item.Color;
			this._skin = item.Skin;
			this._beginDate = item.BeginDate;
			this._attackCompose = item.AttackCompose;
			this._agilityCompose = item.AgilityCompose;
			this._bagType = item.BagType;
			this._isDirty = item.IsDirty;
			this._removeDate = item.RemoveDate;
			this._removeType = item.RemoveType;
			this._hole1 = item.Hole1;
			this._hole2 = item.Hole2;
			this._hole3 = item.Hole3;
			this._hole4 = item.Hole4;
			this._hole5 = item.Hole5;
			this._hole6 = item.Hole6;
			this._hole5Exp = item.Hole5Exp;
			this._hole5Level = item.Hole5Level;
			this._hole6Exp = item.Hole6Exp;
			this._hole6Level = item.Hole6Level;
			this._isGold = item.IsGold;
			this._goldBeginTime = item.goldBeginTime;
			this._goldValidDate = item.goldValidDate;
			this._strengthenExp = item.StrengthenExp;
			this._latentEnergyCurStr = item.latentEnergyCurStr;
			this._latentEnergyNewStr = item._latentEnergyNewStr;
			this._latentEnergyEndTime = item._latentEnergyEndTime;
			this._beadExp = item.beadExp;
			this._beadLevel = item.beadLevel;
			this._beadIsLock = item.beadIsLock;
			this._isShowBind = item.isShowBind;
			this._Damage = item.Damage;
			this._Guard = item.Guard;
			this._Bless = item.Bless;
			this._Blood = item.Blood;
		}
		public bool IsValidItem()
		{
			return this._validDate == 0 || !this._isUsed || DateTime.Compare(this._beginDate.AddDays((double)this._validDate), DateTime.Now) > 0;
		}
		public bool IsValidGoldItem()
		{
			int num = 0;
			if (this._goldValidDate > 0)
			{
				num = DateTime.Compare(this._goldBeginTime.AddDays((double)this._goldValidDate), DateTime.Now);
			}
			return num > 0;
		}
		public bool IsValidLatentEnergy()
		{
			return this._latentEnergyEndTime.Date >= DateTime.Now.Date;
		}
		public void ResetLatentEnergy()
		{
			if (!this.IsValidLatentEnergy())
			{
				this._latentEnergyCurStr = "0,0,0,0";
				this._latentEnergyNewStr = "0,0,0,0";
			}
		}
		public bool CanStackedTo(ItemInfo to)
		{
			return this._templateId == to.TemplateID && this.Template.MaxCount > 1 && this._isBinds == to.IsBinds && this._isUsed == to._isUsed && (this.ValidDate == 0 || (this.BeginDate.Date == to.BeginDate.Date && this.ValidDate == this.ValidDate));
		}
		public int eqType()
		{
			switch (this._template.CategoryID)
			{
			case 51:
				return 1;

			case 52:
				return 2;

			default:
				return 0;
			}
		}
		public bool CanEquip()
		{
			return this._template.CategoryID < 10 || (this._template.CategoryID >= 13 && this._template.CategoryID <= 16);
		}
		public string GetBagName()
		{
			switch (this._template.CategoryID)
			{
			case 10:
			case 11:
				return "Game.Server.GameObjects.Prop";

			case 12:
				return "Game.Server.GameObjects.Task";

			default:
				return "Game.Server.GameObjects.Equip";
			}
		}
		public static ItemInfo CreateFromTemplate(ItemTemplateInfo goods, int count, int type)
		{
			if (goods == null)
			{
				return null;
			}
			return new ItemInfo(goods)
			{
				AgilityCompose = 0,
				AttackCompose = 0,
				BeginDate = DateTime.Now,
				Color = "",
				Skin = "",
				DefendCompose = 0,
				IsUsed = false,
				IsDirty = false,
				IsExist = true,
				IsJudge = true,
				LuckCompose = 0,
				StrengthenLevel = 0,
				TemplateID = goods.TemplateID,
				ValidDate = 0,
				Count = count,
				IsBinds = goods.BindType == 1,
				_removeDate = DateTime.Now,
				_removeType = type,
				Hole1 = -1,
				Hole2 = -1,
				Hole3 = -1,
				Hole4 = -1,
				Hole5 = -1,
				Hole6 = -1,
				Hole5Exp = 0,
				Hole5Level = 0,
				Hole6Exp = 0,
				Hole6Level = 0,
				IsGold = false,
				goldValidDate = 0,
				goldBeginTime = DateTime.Now,
				StrengthenExp = 0,
				latentEnergyCurStr = "0,0,0,0",
				latentEnergyNewStr = "0,0,0,0",
				latentEnergyEndTime = DateTime.Now,
				beadExp = 0,
				beadLevel = 0,
				beadIsLock = false,
				isShowBind = false,
				Damage = 0,
				Guard = 0,
				Bless = 0,
				Blood = 0
			};
		}
		public static ItemInfo CloneFromTemplate(ItemTemplateInfo goods, ItemInfo item)
		{
			if (goods == null)
			{
				return null;
			}
			ItemInfo itemInfo = new ItemInfo(goods);
			itemInfo.AgilityCompose = item.AgilityCompose;
			itemInfo.AttackCompose = item.AttackCompose;
			itemInfo.BeginDate = item.BeginDate;
			itemInfo.Color = item.Color;
			itemInfo.Skin = item.Skin;
			itemInfo.DefendCompose = item.DefendCompose;
			itemInfo.IsBinds = item.IsBinds;
			itemInfo.Place = item.Place;
			itemInfo.BagType = item.BagType;
			itemInfo.IsUsed = item.IsUsed;
			itemInfo.IsDirty = item.IsDirty;
			itemInfo.IsExist = item.IsExist;
			itemInfo.IsJudge = item.IsJudge;
			itemInfo.LuckCompose = item.LuckCompose;
			itemInfo.StrengthenExp = item.StrengthenExp;
			itemInfo.StrengthenLevel = item.StrengthenLevel;
			itemInfo.TemplateID = goods.TemplateID;
			itemInfo.ValidDate = item.ValidDate;
			itemInfo._template = goods;
			itemInfo.Count = item.Count;
			itemInfo._removeDate = item._removeDate;
			itemInfo._removeType = item._removeType;
			itemInfo.Hole1 = item.Hole1;
			itemInfo.Hole2 = item.Hole2;
			itemInfo.Hole3 = item.Hole3;
			itemInfo.Hole4 = item.Hole4;
			itemInfo.Hole5 = item.Hole5;
			itemInfo.Hole6 = item.Hole6;
			itemInfo.Hole5Level = item.Hole5Level;
			itemInfo.Hole5Exp = item.Hole5Exp;
			itemInfo.Hole6Level = item.Hole6Level;
			itemInfo.Hole6Exp = item.Hole6Exp;
			itemInfo.IsGold = item.IsGold;
			itemInfo.goldBeginTime = item.goldBeginTime;
			itemInfo.goldValidDate = item.goldValidDate;
			itemInfo.latentEnergyEndTime = item.latentEnergyEndTime;
			itemInfo.latentEnergyCurStr = item.latentEnergyCurStr;
			itemInfo.latentEnergyNewStr = item.latentEnergyNewStr;
			ItemInfo.OpenHole(ref itemInfo);
			return itemInfo;
		}
		public static void FindSpecialItemInfo(ItemInfo info, ref int gold, ref int money, ref int giftToken, ref int medal)
		{
			int templateID = info.TemplateID;
			if (templateID <= -300)
			{
				if (templateID == -1100)
				{
					giftToken += info.Count;
					info = null;
					return;
				}
				if (templateID != -300)
				{
					return;
				}
				medal += info.Count;
				info = null;
				return;
			}
			else
			{
				if (templateID == -200)
				{
					money += info.Count;
					info = null;
					return;
				}
				if (templateID != -100)
				{
					return;
				}
				gold += info.Count;
				info = null;
				return;
			}
		}
		public static bool SetItemType(ShopItemInfo shop, int type, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int medal)
		{
			if (type == 1)
			{
				ItemInfo.GetItemPrice(shop.APrice1, shop.AValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
				ItemInfo.GetItemPrice(shop.APrice2, shop.AValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
				ItemInfo.GetItemPrice(shop.APrice3, shop.AValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
			}
			if (type == 2)
			{
				ItemInfo.GetItemPrice(shop.BPrice1, shop.BValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
				ItemInfo.GetItemPrice(shop.BPrice2, shop.BValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
				ItemInfo.GetItemPrice(shop.BPrice3, shop.BValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
			}
			if (type == 3)
			{
				ItemInfo.GetItemPrice(shop.CPrice1, shop.CValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
				ItemInfo.GetItemPrice(shop.CPrice2, shop.CValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
				ItemInfo.GetItemPrice(shop.CPrice3, shop.CValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref medal);
			}
			return true;
		}
		public static void GetItemPrice(int Prices, int Values, decimal beat, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int medal)
		{
			switch (Prices)
			{
			case -4:
				offer += (int)(Values * beat);
				return;

			case -3:
				gold += (int)(Values * beat);
				return;

			case -2:
				gifttoken += (int)(Values * beat);
				return;

			case -1:
				money += (int)(Values * beat);
				return;

			default:
				if (Prices != 11408)
				{
					return;
				}
				medal += (int)(Values * beat);
				return;
			}
		}
		public bool IsBead()
		{
			return this._template.Property1 == 31 && this._template.CategoryID == 11;
		}
		public bool IsEquipPet()
		{
			return this._template.CategoryID == 50 || this._template.CategoryID == 51 || this._template.CategoryID == 52;
		}
		public static void OpenHole(ref ItemInfo item)
		{
			string[] array = item.Template.Hole.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (item.StrengthenLevel >= Convert.ToInt32(array2[0]) && Convert.ToInt32(array2[1]) != -1)
				{
					switch (i)
					{
					case 0:
						if (item.Hole1 < 0)
						{
							item.Hole1 = 0;
						}
						break;

					case 1:
						if (item.Hole2 < 0)
						{
							item.Hole2 = 0;
						}
						break;

					case 2:
						if (item.Hole3 < 0)
						{
							item.Hole3 = 0;
						}
						break;

					case 3:
						if (item.Hole4 < 0)
						{
							item.Hole4 = 0;
						}
						break;

					case 4:
						if (item.Hole5 < 0)
						{
							item.Hole5 = 0;
						}
						break;

					case 5:
						if (item.Hole6 < 0)
						{
							item.Hole6 = 0;
						}
						break;
					}
				}
			}
		}
	}
}
