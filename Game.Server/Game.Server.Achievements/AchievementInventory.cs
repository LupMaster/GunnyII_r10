using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server.Achievements
{
	public class AchievementInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private object m_lock;
		protected List<BaseAchievement> m_list;
		protected List<AchievementDataInfo> m_datas;
		protected ArrayList m_clearList;
		private GamePlayer m_player;
		private byte[] m_states;
		private UnicodeEncoding m_converter;
		protected List<BaseAchievement> m_changedAchievements = new List<BaseAchievement>();
		private int m_changeCount;
		public AchievementInventory(GamePlayer player)
		{
			this.m_converter = new UnicodeEncoding();
			this.m_player = player;
			this.m_lock = new object();
			this.m_list = new List<BaseAchievement>();
			this.m_clearList = new ArrayList();
			this.m_datas = new List<AchievementDataInfo>();
		}
		public void LoadFromDatabase(int playerId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_states = this.InitAchievement();
				using (ProduceBussiness produceBussiness = new ProduceBussiness())
				{
					AchievementDataInfo[] allAchievementData = produceBussiness.GetAllAchievementData(playerId);
					this.BeginChanges();
					AchievementDataInfo[] array = allAchievementData;
					for (int i = 0; i < array.Length; i++)
					{
						AchievementDataInfo achievementDataInfo = array[i];
						AchievementInfo singleAchievement = AchievementMgr.GetSingleAchievement(achievementDataInfo.AchievementID);
						if (singleAchievement != null)
						{
							this.AddAchievement(new BaseAchievement(singleAchievement, achievementDataInfo));
						}
						this.AddAchievementData(achievementDataInfo);
					}
					this.CommitChanges();
				}
				List<BaseAchievement> arg_87_0 = this.m_list;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		private byte[] InitAchievement()
		{
			byte[] array = new byte[200];
			for (int i = 0; i < 200; i++)
			{
				array[i] = 0;
			}
			return array;
		}
		protected void OnQuestsChanged(BaseAchievement quest)
		{
			if (!this.m_changedAchievements.Contains(quest))
			{
				this.m_changedAchievements.Add(quest);
			}
			if (this.m_changeCount <= 0 && this.m_changedAchievements.Count > 0)
			{
				this.UpdateChangedQuests();
			}
		}
		private bool AddAchievement(BaseAchievement achivev)
		{
			List<BaseAchievement> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				this.m_list.Add(achivev);
			}
			finally
			{
				Monitor.Exit(list);
			}
			this.OnQuestsChanged(achivev);
			achivev.AddToPlayer(this.m_player);
			return true;
		}
		public void Update(BaseAchievement achieve)
		{
			this.OnQuestsChanged(achieve);
		}
		private bool AddAchievementData(AchievementDataInfo data)
		{
			List<BaseAchievement> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				this.m_datas.Add(data);
			}
			finally
			{
				Monitor.Exit(list);
			}
			return true;
		}
		public void SaveToDatabase()
		{
		}
		private void BeginChanges()
		{
			Interlocked.Increment(ref this.m_changeCount);
		}
		private void CommitChanges()
		{
			int num = Interlocked.Decrement(ref this.m_changeCount);
			if (num < 0)
			{
				if (AchievementInventory.log.IsErrorEnabled)
				{
					AchievementInventory.log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref this.m_changeCount, 0);
			}
			if (num <= 0 && this.m_changedAchievements.Count > 0)
			{
				this.UpdateChangedQuests();
			}
		}
		public void UpdateChangedQuests()
		{
			this.m_player.Out.SendAchievementDatas(this.m_player, this.m_changedAchievements.ToArray());
			this.m_changedAchievements.Clear();
		}
	}
}
