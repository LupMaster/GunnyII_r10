using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Achievements
{
	public class BaseAchievement
	{
		private AchievementInfo m_info;
		private AchievementDataInfo m_data;
		private List<BaseCondition> m_list;
		private string m_rank;
		private GamePlayer m_player;
		public string Rank
		{
			get
			{
				return this.m_rank;
			}
		}
		public AchievementInfo Info
		{
			get
			{
				return this.m_info;
			}
		}
		public AchievementDataInfo Data
		{
			get
			{
				return this.m_data;
			}
		}
		public BaseAchievement(AchievementInfo info, AchievementDataInfo data)
		{
			this.m_info = info;
			this.m_data = data;
			this.m_data.AchievementID = this.m_info.ID;
			this.m_rank = this.m_info.Title;
			this.m_list = new List<BaseCondition>();
		}
		public void AddToPlayer(GamePlayer player)
		{
			this.m_player = player;
			if (!this.m_data.IsComplete)
			{
				this.AddTrigger(player);
			}
		}
		private void AddTrigger(GamePlayer player)
		{
			foreach (BaseCondition current in this.m_list)
			{
				current.AddTrigger(player);
			}
		}
		public void SaveData()
		{
			foreach (BaseCondition arg_15_0 in this.m_list)
			{
			}
		}
		public void Update()
		{
			this.SaveData();
			if (this.m_data.IsDirty && this.m_player != null)
			{
				this.m_player.AchievementInventory.Update(this);
			}
		}
	}
}
