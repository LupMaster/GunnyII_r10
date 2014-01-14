using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Server.GameUtils
{
	public class PlayerProperty
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected GamePlayer m_player;
		private Dictionary<string, Dictionary<string, int>> m_playerProperty;
		private Dictionary<string, Dictionary<string, int>> m_otherPlayerProperty;
		protected int m_loading;
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
		}
		public Dictionary<string, Dictionary<string, int>> CurrentPlayerProperty
		{
			get
			{
				return this.m_playerProperty;
			}
			set
			{
				this.m_playerProperty = value;
			}
		}
		public Dictionary<string, Dictionary<string, int>> OtherPlayerProperty
		{
			get
			{
				return this.m_playerProperty;
			}
			set
			{
				this.m_playerProperty = value;
			}
		}
		public int Loading
		{
			get
			{
				return this.m_loading;
			}
			set
			{
				this.m_loading = value;
			}
		}
		public PlayerProperty(GamePlayer player)
		{
			this.m_player = player;
			this.m_playerProperty = new Dictionary<string, Dictionary<string, int>>();
			this.m_otherPlayerProperty = new Dictionary<string, Dictionary<string, int>>();
			this.m_loading = 0;
		}
		public void AddProp(string key, Dictionary<string, int> propAdd)
		{
			if (!this.m_playerProperty.ContainsKey(key))
			{
				this.m_playerProperty.Add(key, propAdd);
				return;
			}
			this.m_playerProperty[key] = propAdd;
		}
		public void AddOtherProp(string key, Dictionary<string, int> propAdd)
		{
			if (!this.m_playerProperty.ContainsKey(key))
			{
				this.m_otherPlayerProperty.Add(key, propAdd);
				return;
			}
			this.m_otherPlayerProperty[key] = propAdd;
		}
		public void ViewCurrent()
		{
			this.m_player.Out.SendUpdatePlayerProperty(this.m_player.PlayerCharacter, this.m_playerProperty);
		}
		public void ViewOther(PlayerInfo player)
		{
			this.CreateProp(false, "Texp", 0, 0, 0, 0, 0);
			this.CreateProp(false, "Card", 0, 0, 0, 0, 0);
			this.CreateProp(false, "Pet", 0, 0, 0, 0, 0);
			this.CreateProp(false, "Gem", 0, 0, 0, 0, 0);
			double num = 0.0;
			double num2 = 0.0;
			this.CreateSuitBonus(false, ref num2, ref num);
			this.CreateBeadProp(false, 0.0, 0.0);
			this.m_player.Out.SendUpdatePlayerProperty(player, this.m_otherPlayerProperty);
		}
		public void CreateSuitBonus(bool isSelf, ref double basedefence, ref double baseattack)
		{
			int num = 0;
			int num2 = 0;
			int value = 0;
			int value2 = 0;
			int value3 = 0;
			int value4 = 0;
			int value5 = 0;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			basedefence += (double)num;
			baseattack += (double)num2;
			dictionary.Add("Attack", value);
			dictionary.Add("Defence", value2);
			dictionary.Add("Agility", value3);
			dictionary.Add("Luck", value4);
			dictionary.Add("HP", value5);
			dictionary.Add("Damage", num);
			dictionary.Add("Guard", num2);
			if (isSelf)
			{
				this.AddProp("Suit", dictionary);
				return;
			}
			this.AddOtherProp("Suit", dictionary);
		}
		public void CreateBeadProp(bool isSelf, double basedefence, double baseattack)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("Damage", (int)baseattack);
			dictionary.Add("Armor", (int)basedefence);
			if (isSelf)
			{
				this.AddProp("Bead", dictionary);
				return;
			}
			this.AddOtherProp("Bead", dictionary);
		}
		public void CreateProp(bool isSelf, string skey, int attack, int defence, int agility, int lucky, int hp)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("Attack", attack);
			dictionary.Add("Defence", defence);
			dictionary.Add("Agility", agility);
			dictionary.Add("Luck", lucky);
			dictionary.Add("HP", hp);
			if (isSelf)
			{
				this.AddProp(skey, dictionary);
				return;
			}
			this.AddOtherProp(skey, dictionary);
		}
	}
}
