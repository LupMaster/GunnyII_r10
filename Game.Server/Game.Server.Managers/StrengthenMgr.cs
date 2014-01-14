using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class StrengthenMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, StrengthenInfo> _strengthens;
		private static Dictionary<int, StrengthenInfo> _Refinery_Strengthens;
		private static Dictionary<int, StrengthenGoodsInfo> _Strengthens_Goods;
		private static Dictionary<int, StrengThenExpInfo> _Strengthens_Exps;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, StrengthenInfo> dictionary = new Dictionary<int, StrengthenInfo>();
				Dictionary<int, StrengthenInfo> dictionary2 = new Dictionary<int, StrengthenInfo>();
				Dictionary<int, StrengThenExpInfo> dictionary3 = new Dictionary<int, StrengThenExpInfo>();
				Dictionary<int, StrengthenGoodsInfo> dictionary4 = new Dictionary<int, StrengthenGoodsInfo>();
				if (StrengthenMgr.LoadStrengthen(dictionary, dictionary2, dictionary3, dictionary4))
				{
					StrengthenMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						StrengthenMgr._strengthens = dictionary;
						StrengthenMgr._Refinery_Strengthens = dictionary2;
						StrengthenMgr._Strengthens_Exps = dictionary3;
						StrengthenMgr._Strengthens_Goods = dictionary4;
						return true;
					}
					catch
					{
					}
					finally
					{
						StrengthenMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (StrengthenMgr.log.IsErrorEnabled)
				{
					StrengthenMgr.log.Error("StrengthenMgr", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				StrengthenMgr.m_lock = new ReaderWriterLock();
				StrengthenMgr._strengthens = new Dictionary<int, StrengthenInfo>();
				StrengthenMgr._Refinery_Strengthens = new Dictionary<int, StrengthenInfo>();
				StrengthenMgr._Strengthens_Goods = new Dictionary<int, StrengthenGoodsInfo>();
				StrengthenMgr._Strengthens_Exps = new Dictionary<int, StrengThenExpInfo>();
				StrengthenMgr.rand = new ThreadSafeRandom();
				result = StrengthenMgr.LoadStrengthen(StrengthenMgr._strengthens, StrengthenMgr._Refinery_Strengthens, StrengthenMgr._Strengthens_Exps, StrengthenMgr._Strengthens_Goods);
			}
			catch (Exception exception)
			{
				if (StrengthenMgr.log.IsErrorEnabled)
				{
					StrengthenMgr.log.Error("StrengthenMgr", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadStrengthen(Dictionary<int, StrengthenInfo> strengthen, Dictionary<int, StrengthenInfo> RefineryStrengthen, Dictionary<int, StrengThenExpInfo> StrengthenExp, Dictionary<int, StrengthenGoodsInfo> StrengthensGoods)
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				StrengthenInfo[] allStrengthen = produceBussiness.GetAllStrengthen();
				StrengthenInfo[] allRefineryStrengthen = produceBussiness.GetAllRefineryStrengthen();
				StrengThenExpInfo[] allStrengThenExp = produceBussiness.GetAllStrengThenExp();
				StrengthenGoodsInfo[] allStrengthenGoodsInfo = produceBussiness.GetAllStrengthenGoodsInfo();
				StrengthenInfo[] array = allStrengthen;
				for (int i = 0; i < array.Length; i++)
				{
					StrengthenInfo strengthenInfo = array[i];
					if (!strengthen.ContainsKey(strengthenInfo.StrengthenLevel))
					{
						strengthen.Add(strengthenInfo.StrengthenLevel, strengthenInfo);
					}
				}
				StrengthenInfo[] array2 = allRefineryStrengthen;
				for (int j = 0; j < array2.Length; j++)
				{
					StrengthenInfo strengthenInfo2 = array2[j];
					if (!RefineryStrengthen.ContainsKey(strengthenInfo2.StrengthenLevel))
					{
						RefineryStrengthen.Add(strengthenInfo2.StrengthenLevel, strengthenInfo2);
					}
				}
				StrengThenExpInfo[] array3 = allStrengThenExp;
				for (int k = 0; k < array3.Length; k++)
				{
					StrengThenExpInfo strengThenExpInfo = array3[k];
					if (!StrengthenExp.ContainsKey(strengThenExpInfo.Level))
					{
						StrengthenExp.Add(strengThenExpInfo.Level, strengThenExpInfo);
					}
				}
				StrengthenGoodsInfo[] array4 = allStrengthenGoodsInfo;
				for (int l = 0; l < array4.Length; l++)
				{
					StrengthenGoodsInfo strengthenGoodsInfo = array4[l];
					if (!StrengthensGoods.ContainsKey(strengthenGoodsInfo.ID))
					{
						StrengthensGoods.Add(strengthenGoodsInfo.ID, strengthenGoodsInfo);
					}
				}
			}
			return true;
		}
		public static StrengThenExpInfo FindStrengthenExpInfo(int level)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (StrengthenMgr._Strengthens_Exps.ContainsKey(level))
				{
					return StrengthenMgr._Strengthens_Exps[level];
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static bool canUpLv(int exp, int level)
		{
			StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(level + 1);
			return strengThenExpInfo != null && exp >= strengThenExpInfo.Exp;
		}
		public static int getNeedExp(int Exp, int level)
		{
			StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(level + 1);
			if (strengThenExpInfo == null)
			{
				return 0;
			}
			return strengThenExpInfo.Exp - Exp;
		}
		public static int GetNecklacePlus(int exp, int currentPlus)
		{
			foreach (StrengThenExpInfo current in StrengthenMgr._Strengthens_Exps.Values)
			{
				if (exp < current.NecklaceStrengthExp)
				{
					int necklaceLevel = StrengthenMgr.GetNecklaceLevel(exp);
					StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(necklaceLevel);
					int result;
					if (strengThenExpInfo == null)
					{
						result = currentPlus;
						return result;
					}
					result = strengThenExpInfo.NecklaceStrengthPlus;
					return result;
				}
			}
			return currentPlus;
		}
		public static int GetNecklaceLevel(int exp)
		{
			foreach (StrengThenExpInfo current in StrengthenMgr._Strengthens_Exps.Values)
			{
				if (exp < current.NecklaceStrengthExp)
				{
					return (current.Level - 1 < 0) ? 0 : (current.Level - 1);
				}
			}
			return 0;
		}
		public static int GetNecklaceMaxExp(int lv)
		{
			StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(lv);
			if (strengThenExpInfo == null)
			{
				return 0;
			}
			return strengThenExpInfo.NecklaceStrengthExp;
		}
		public static int GetNecklaceMaxPlus(int lv)
		{
			StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(lv);
			if (strengThenExpInfo == null)
			{
				return 0;
			}
			return strengThenExpInfo.NecklaceStrengthPlus;
		}
		public static StrengthenInfo FindStrengthenInfo(int level)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (StrengthenMgr._strengthens.ContainsKey(level))
				{
					return StrengthenMgr._strengthens[level];
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static StrengthenInfo FindRefineryStrengthenInfo(int level)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (StrengthenMgr._Refinery_Strengthens.ContainsKey(level))
				{
					return StrengthenMgr._Refinery_Strengthens[level];
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static StrengthenGoodsInfo FindStrengthenGoodsInfo(int level, int templateId)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (int current in StrengthenMgr._Strengthens_Goods.Keys)
				{
					if (StrengthenMgr._Strengthens_Goods[current].Level == level && templateId == StrengthenMgr._Strengthens_Goods[current].CurrentEquip)
					{
						return StrengthenMgr._Strengthens_Goods[current];
					}
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static void InheritProperty(ItemInfo Item, ref ItemInfo item)
		{
			if (Item.Hole1 >= 0)
			{
				item.Hole1 = Item.Hole1;
			}
			if (Item.Hole2 >= 0)
			{
				item.Hole2 = Item.Hole2;
			}
			if (Item.Hole3 >= 0)
			{
				item.Hole3 = Item.Hole3;
			}
			if (Item.Hole4 >= 0)
			{
				item.Hole4 = Item.Hole4;
			}
			if (Item.Hole5 >= 0)
			{
				item.Hole5 = Item.Hole5;
			}
			if (Item.Hole6 >= 0)
			{
				item.Hole6 = Item.Hole6;
			}
			item.AttackCompose = Item.AttackCompose;
			item.DefendCompose = Item.DefendCompose;
			item.LuckCompose = Item.LuckCompose;
			item.AgilityCompose = Item.AgilityCompose;
			item.IsBinds = Item.IsBinds;
			item.ValidDate = Item.ValidDate;
		}
		public static void InheritTransferProperty(ref ItemInfo itemZero, ref ItemInfo itemOne, bool tranHole, bool tranHoleFivSix)
		{
			int hole = itemZero.Hole1;
			int hole2 = itemZero.Hole2;
			int hole3 = itemZero.Hole3;
			int hole4 = itemZero.Hole4;
			int hole5 = itemZero.Hole5;
			int hole6 = itemZero.Hole6;
			int hole5Exp = itemZero.Hole5Exp;
			int hole5Level = itemZero.Hole5Level;
			int hole6Exp = itemZero.Hole6Exp;
			int hole6Level = itemZero.Hole6Level;
			int attackCompose = itemZero.AttackCompose;
			int defendCompose = itemZero.DefendCompose;
			int agilityCompose = itemZero.AgilityCompose;
			int luckCompose = itemZero.LuckCompose;
			int strengthenLevel = itemZero.StrengthenLevel;
			int strengthenExp = itemZero.StrengthenExp;
			bool isGold = itemZero.IsGold;
			int goldValidDate = itemZero.goldValidDate;
			DateTime goldBeginTime = itemZero.goldBeginTime;
			string latentEnergyCurStr = itemZero.latentEnergyCurStr;
			string latentEnergyNewStr = itemZero.latentEnergyNewStr;
			DateTime latentEnergyEndTime = itemZero.latentEnergyEndTime;
			int hole7 = itemOne.Hole1;
			int hole8 = itemOne.Hole2;
			int hole9 = itemOne.Hole3;
			int hole10 = itemOne.Hole4;
			int hole11 = itemOne.Hole5;
			int hole12 = itemOne.Hole6;
            int hole13 = itemOne.Hole6;
            int hole14 = itemOne.Hole6;
            int hole15 = itemOne.Hole6;
			int hole5Exp2 = itemOne.Hole5Exp;
			int hole5Level2 = itemOne.Hole5Level;
			int hole6Exp2 = itemOne.Hole6Exp;
			int hole6Level2 = itemOne.Hole6Level;
			int attackCompose2 = itemOne.AttackCompose;
			int defendCompose2 = itemOne.DefendCompose;
			int agilityCompose2 = itemOne.AgilityCompose;
			int luckCompose2 = itemOne.LuckCompose;
			int strengthenLevel2 = itemOne.StrengthenLevel;
			int strengthenExp2 = itemOne.StrengthenExp;
			bool isGold2 = itemOne.IsGold;
			int goldValidDate2 = itemOne.goldValidDate;
			DateTime goldBeginTime2 = itemOne.goldBeginTime;
			string latentEnergyCurStr2 = itemOne.latentEnergyCurStr;
			string latentEnergyNewStr2 = itemOne.latentEnergyNewStr;
			DateTime latentEnergyEndTime2 = itemOne.latentEnergyEndTime;
			if (tranHole)
			{
				itemOne.Hole1 = hole;
				itemZero.Hole1 = hole7;
				itemOne.Hole2 = hole2;
				itemZero.Hole2 = hole8;
				itemOne.Hole3 = hole3;
				itemZero.Hole3 = hole9;
				itemOne.Hole4 = hole4;
				itemZero.Hole4 = hole10;
			}
			if (tranHoleFivSix)
			{
				itemOne.Hole5 = hole5;
				itemZero.Hole5 = hole11;
				itemOne.Hole6 = hole6;
				itemZero.Hole6 = hole12;
			}
			itemOne.Hole5Exp = hole5Exp;
			itemZero.Hole5Exp = hole5Exp2;
			itemOne.Hole5Level = hole5Level;
			itemZero.Hole5Level = hole5Level2;
			itemOne.Hole6Exp = hole6Exp;
			itemZero.Hole6Exp = hole6Exp2;
			itemOne.Hole6Level = hole6Level;
			itemZero.Hole6Level = hole6Level2;
			itemZero.StrengthenLevel = strengthenLevel2;
			itemOne.StrengthenLevel = strengthenLevel;
			itemZero.StrengthenExp = strengthenExp2;
			itemOne.StrengthenExp = strengthenExp;
			itemZero.AttackCompose = attackCompose2;
			itemOne.AttackCompose = attackCompose;
			itemZero.DefendCompose = defendCompose2;
			itemOne.DefendCompose = defendCompose;
			itemZero.LuckCompose = luckCompose2;
			itemOne.LuckCompose = luckCompose;
			itemZero.AgilityCompose = agilityCompose2;
			itemOne.AgilityCompose = agilityCompose;
			if (itemZero.IsBinds || itemOne.IsBinds)
			{
				itemOne.IsBinds = true;
				itemZero.IsBinds = true;
			}
			itemZero.IsGold = isGold2;
			itemOne.IsGold = isGold;
			itemZero.goldBeginTime = goldBeginTime2;
			itemOne.goldBeginTime = goldBeginTime;
			itemZero.goldValidDate = goldValidDate2;
			itemOne.goldValidDate = goldValidDate;
			itemZero.latentEnergyCurStr = latentEnergyCurStr2;
			itemOne.latentEnergyCurStr = latentEnergyCurStr;
			itemZero.latentEnergyNewStr = latentEnergyNewStr2;
			itemOne.latentEnergyNewStr = latentEnergyNewStr;
			itemZero.latentEnergyEndTime = latentEnergyEndTime2;
			itemOne.latentEnergyEndTime = latentEnergyEndTime;
		}
	}
}
