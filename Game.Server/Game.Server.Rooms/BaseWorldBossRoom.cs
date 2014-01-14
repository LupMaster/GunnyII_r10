using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseWorldBossRoom
	{
		private long MAX_BLOOD = 100000000000L;
		private Dictionary<int, GamePlayer> m_list;
		private Dictionary<string, RankingPersonInfo> m_rankList;
		private long m_blood;
		private long m_lostBlood;
		private bool m_die;
		public long Blood
		{
			get
			{
				return this.m_blood;
			}
			set
			{
				this.m_blood = value;
			}
		}
		public bool IsDie
		{
			get
			{
				return this.m_die;
			}
			set
			{
				this.m_die = value;
			}
		}
		public long MaxBlood
		{
			get
			{
				return this.MAX_BLOOD;
			}
		}
		public BaseWorldBossRoom()
		{
			this.m_list = new Dictionary<int, GamePlayer>();
			this.m_rankList = new Dictionary<string, RankingPersonInfo>();
			this.m_blood = this.MAX_BLOOD;
			this.m_die = false;
		}
		public void UpdateRank(int damage, int honor, string NickName)
		{
			if (!this.m_rankList.Keys.Contains(NickName))
			{
				RankingPersonInfo rankingPersonInfo = new RankingPersonInfo();
				rankingPersonInfo.ID = this.m_rankList.Count + 1;
				rankingPersonInfo.Name = NickName;
				rankingPersonInfo.Damage = damage;
				rankingPersonInfo.Honor = honor;
				this.m_rankList.Add(NickName, rankingPersonInfo);
				return;
			}
			this.m_rankList[NickName].Damage += damage;
			this.m_rankList[NickName].Honor += honor;
		}
		public bool CheckName(string NickName)
		{
			return this.m_rankList.Keys.Contains(NickName);
		}
		public void ReduceBlood(int value)
		{
			this.m_blood -= (long)value;
			this.m_lostBlood += (long)value;
			if (this.m_blood < 0L)
			{
				this.m_die = true;
			}
			if (this.m_lostBlood > 5000000L)
			{
				this.SendUpdateBlood();
				this.m_lostBlood = 0L;
			}
		}
		private List<RankingPersonInfo> SelectTopTen()
		{
			List<RankingPersonInfo> list = new List<RankingPersonInfo>();
			List<RankingPersonInfo> list2 = (
				from s in this.m_rankList.Values
				orderby s.Damage
				select s).ToList<RankingPersonInfo>();
			foreach (RankingPersonInfo current in list2)
			{
				if (list.Count == 10)
				{
					break;
				}
				list.Add(current);
			}
			return list;
		}
		public void SendUpdateRank()
		{
			int count = this.SelectTopTen().Count;
			GSPacketIn gSPacketIn = new GSPacketIn(102);
			gSPacketIn.WriteByte(10);
			gSPacketIn.WriteBoolean(false);
			gSPacketIn.WriteInt(count);
			foreach (RankingPersonInfo current in this.SelectTopTen())
			{
				gSPacketIn.WriteInt(current.ID);
				gSPacketIn.WriteString(current.Name);
				gSPacketIn.WriteInt(current.Damage);
			}
			this.SendToALL(gSPacketIn);
		}
		public void SendPrivateInfo(GamePlayer player)
		{
			string nickName = player.PlayerCharacter.NickName;
			if (!this.CheckName(nickName))
			{
				return;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(102);
			gSPacketIn.WriteByte(22);
			RankingPersonInfo rankingPersonInfo = this.m_rankList[nickName];
			gSPacketIn.WriteInt(rankingPersonInfo.Damage);
			gSPacketIn.WriteInt(rankingPersonInfo.Honor);
			player.Out.SendTCP(gSPacketIn);
		}
		public void SendUpdateBlood()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(102);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteBoolean(false);
			gSPacketIn.WriteLong(this.MAX_BLOOD);
			gSPacketIn.WriteLong(this.m_blood);
			this.SendToALL(gSPacketIn);
		}
		public bool AddPlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				if (!this.m_list.ContainsKey(player.PlayerId))
				{
					player.IsInWorldBossRoom = true;
					this.m_list.Add(player.PlayerId, player);
					flag = true;
					this.SendUpdateRank();
					if (this.m_blood < this.MAX_BLOOD)
					{
						this.SendUpdateBlood();
					}
				}
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (flag)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(102);
				gSPacketIn.WriteByte(3);
				gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
				gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
				gSPacketIn.WriteInt(player.PlayerCharacter.ID);
				gSPacketIn.WriteString(player.PlayerCharacter.NickName);
				gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
				gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
				gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
				gSPacketIn.WriteString(player.PlayerCharacter.Style);
				gSPacketIn.WriteString(player.PlayerCharacter.Colors);
				gSPacketIn.WriteString(player.PlayerCharacter.Skin);
				gSPacketIn.WriteInt(player.WorldBossX);
				gSPacketIn.WriteInt(player.WorldBossY);
				gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
				gSPacketIn.WriteInt(player.PlayerCharacter.Win);
				gSPacketIn.WriteInt(player.PlayerCharacter.Total);
				gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
				gSPacketIn.WriteByte(player.WorldBossState);
				player.SendTCP(gSPacketIn);
				this.SendToALL(gSPacketIn, player);
			}
			return flag;
		}
		public void UpdateRoom(GamePlayer player)
		{
			GamePlayer[] playersSafe = this.GetPlayersSafe();
			GamePlayer[] array = playersSafe;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer != player)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(102);
					gSPacketIn.WriteByte(3);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Grade);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Hide);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Repute);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
					gSPacketIn.WriteString(gamePlayer.PlayerCharacter.NickName);
					gSPacketIn.WriteByte(gamePlayer.PlayerCharacter.typeVIP);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
					gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
					gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Style);
					gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Colors);
					gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Skin);
					gSPacketIn.WriteInt(gamePlayer.WorldBossX);
					gSPacketIn.WriteInt(gamePlayer.WorldBossY);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
					gSPacketIn.WriteByte(gamePlayer.WorldBossState);
					player.SendTCP(gSPacketIn);
				}
			}
		}
		public bool RemovePlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				flag = this.m_list.Remove(player.PlayerId);
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (flag)
			{
				GSPacketIn packet = player.Out.SendSceneRemovePlayer(player);
				this.SendToALL(packet, player);
			}
			return true;
		}
		public GamePlayer[] GetPlayersSafe()
		{
			GamePlayer[] array = null;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				array = new GamePlayer[this.m_list.Count];
				this.m_list.Values.CopyTo(array, 0);
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (array != null)
			{
				return array;
			}
			return new GamePlayer[0];
		}
		public void SendToALL(GSPacketIn packet)
		{
			this.SendToALL(packet, null);
		}
		public void SendToALL(GSPacketIn packet, GamePlayer except)
		{
			GamePlayer[] array = null;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				array = new GamePlayer[this.m_list.Count];
				this.m_list.Values.CopyTo(array, 0);
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (array != null)
			{
				GamePlayer[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					GamePlayer gamePlayer = array2[i];
					if (gamePlayer != null && gamePlayer != except)
					{
						gamePlayer.Out.SendTCP(packet);
					}
				}
			}
		}
	}
}
