using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Reflection;
namespace Game.Server.Achievements
{
	public class BaseCondition
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected AchievementConditionInfo m_info;
		private int m_value;
		private BaseAchievement m_quest;
		public AchievementConditionInfo Info
		{
			get
			{
				return this.m_info;
			}
		}
		public int Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				if (this.m_value != value)
				{
					this.m_value = value;
					this.m_quest.Update();
				}
			}
		}
		public BaseCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		{
			this.m_quest = quest;
			this.m_info = info;
			this.m_value = value;
		}
		public virtual void Reset(GamePlayer player)
		{
			this.m_value = this.m_info.Condiction_Para2;
		}
		public virtual void AddTrigger(GamePlayer player)
		{
		}
		public virtual void RemoveTrigger(GamePlayer player)
		{
		}
		public virtual bool IsCompleted(GamePlayer player)
		{
			return false;
		}
		public virtual bool Finish(GamePlayer player)
		{
			return true;
		}
		public virtual bool CancelFinish(GamePlayer player)
		{
			return true;
		}
	}
}
