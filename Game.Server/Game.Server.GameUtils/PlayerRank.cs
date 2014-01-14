using Bussiness;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class PlayerRank
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected GamePlayer m_player;
		private List<UserRankInfo> m_rank;
		private UserRankInfo m_currentRank;
		private bool m_saveToDb;
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
		}
		public List<UserRankInfo> Ranks
		{
			get
			{
				return this.m_rank;
			}
			set
			{
				this.m_rank = value;
			}
		}
		public UserRankInfo CurrentRank
		{
			get
			{
				return this.m_currentRank;
			}
			set
			{
				this.m_currentRank = value;
			}
		}
		public PlayerRank(GamePlayer player, bool saveTodb)
		{
			this.m_player = player;
			this.m_saveToDb = saveTodb;
			this.m_rank = new List<UserRankInfo>();
			this.m_currentRank = this.GetRank(this.m_player.PlayerCharacter.Honor);
		}
		public virtual void LoadFromDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					List<UserRankInfo> singleUserRank = playerBussiness.GetSingleUserRank(this.Player.PlayerCharacter.ID);
					if (singleUserRank.Count == 0)
					{
						this.CreateRank(this.Player.PlayerCharacter.ID);
					}
					else
					{
						foreach (UserRankInfo current in singleUserRank)
						{
							if (current.IsValidRank())
							{
								this.AddRank(current);
							}
							else
							{
								this.RemoveRank(current);
							}
						}
					}
				}
			}
		}
		public void AddRank(UserRankInfo info)
		{
			List<UserRankInfo> rank;
			Monitor.Enter(rank = this.m_rank);
			try
			{
				this.m_rank.Add(info);
			}
			finally
			{
				Monitor.Exit(rank);
			}
		}
		public void RemoveRank(UserRankInfo item)
		{
			item.IsExit = false;
			this.AddRank(item);
		}
		public List<UserRankInfo> GetRank()
		{
			List<UserRankInfo> list = new List<UserRankInfo>();
			foreach (UserRankInfo current in this.m_rank)
			{
				if (current.IsExit)
				{
					list.Add(current);
				}
			}
			return list;
		}
		public UserRankInfo GetRank(string honor)
		{
			foreach (UserRankInfo current in this.m_rank)
			{
				if (current.UserRank == honor)
				{
					return current;
				}
			}
			return null;
		}
		public bool IsRank(string honor)
		{
			foreach (UserRankInfo current in this.m_rank)
			{
				if (current.UserRank == honor)
				{
					return true;
				}
			}
			return false;
		}
		public void CreateRank(int UserID)
		{
			new List<UserRankInfo>();
			this.AddRank(new UserRankInfo
			{
				ID = 0,
				UserID = UserID,
				UserRank = "Bé tập chơi",
				Attack = 0,
				Defence = 0,
				Luck = 0,
				Agility = 0,
				HP = 0,
				Damage = 0,
				Guard = 0,
				BeginDate = DateTime.Now,
				Validate = 0,
				IsExit = true
			});
		}
		public virtual void SaveToDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					List<UserRankInfo> rank;
					Monitor.Enter(rank = this.m_rank);
					try
					{
						for (int i = 0; i < this.m_rank.Count; i++)
						{
							UserRankInfo userRankInfo = this.m_rank[i];
							if (userRankInfo != null && userRankInfo.IsDirty)
							{
								if (userRankInfo.ID > 0)
								{
									playerBussiness.UpdateUserRank(userRankInfo);
								}
								else
								{
									playerBussiness.AddUserRank(userRankInfo);
								}
							}
						}
					}
					finally
					{
						Monitor.Exit(rank);
					}
				}
			}
		}
	}
}
