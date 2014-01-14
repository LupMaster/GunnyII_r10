using Bussiness;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class BallMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, BallInfo> m_infos;
		private static Dictionary<int, Tile> m_tiles;
		public static bool Init()
		{
			return BallMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, BallInfo> dictionary = BallMgr.LoadFromDatabase();
				Dictionary<int, Tile> dictionary2 = BallMgr.LoadFromFiles(dictionary);
				if (dictionary.Values.Count > 0 && dictionary2.Values.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, BallInfo>>(ref BallMgr.m_infos, dictionary);
					Interlocked.Exchange<Dictionary<int, Tile>>(ref BallMgr.m_tiles, dictionary2);
					return true;
				}
			}
			catch (Exception exception)
			{
				BallMgr.log.Error("Ball Mgr init error:", exception);
			}
			return false;
		}
		private static Dictionary<int, BallInfo> LoadFromDatabase()
		{
			Dictionary<int, BallInfo> dictionary = new Dictionary<int, BallInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				BallInfo[] allBall = produceBussiness.GetAllBall();
				BallInfo[] array = allBall;
				for (int i = 0; i < array.Length; i++)
				{
					BallInfo ballInfo = array[i];
					if (!dictionary.ContainsKey(ballInfo.ID))
					{
						dictionary.Add(ballInfo.ID, ballInfo);
					}
				}
			}
			return dictionary;
		}
		private static Dictionary<int, Tile> LoadFromFiles(Dictionary<int, BallInfo> list)
		{
			Dictionary<int, Tile> dictionary = new Dictionary<int, Tile>();
			foreach (BallInfo current in list.Values)
			{
				if (current.HasTunnel)
				{
					string text = string.Format("bomb\\{0}.bomb", current.ID);
					Tile tile = null;
					if (File.Exists(text))
					{
						tile = new Tile(text, false);
					}
					dictionary.Add(current.ID, tile);
					if (tile == null && current.ID != 1 && current.ID != 2 && current.ID != 3)
					{
						BallMgr.log.ErrorFormat("can't find bomb file:{0}", text);
					}
				}
			}
			return dictionary;
		}
		public static BallInfo FindBall(int id)
		{
			if (BallMgr.m_infos.ContainsKey(id))
			{
				return BallMgr.m_infos[id];
			}
			return null;
		}
		public static Tile FindTile(int id)
		{
			if (BallMgr.m_tiles.ContainsKey(id))
			{
				return BallMgr.m_tiles[id];
			}
			return null;
		}
		public static BombType GetBallType(int ballId)
		{
			if (ballId <= 59)
			{
				switch (ballId)
				{
				case 1:
					break;

				case 2:
				case 4:
					return BombType.Normal;

				case 3:
					return BombType.FLY;

				case 5:
					return BombType.CURE;

				default:
					if (ballId != 56)
					{
						if (ballId != 59)
						{
							return BombType.Normal;
						}
						return BombType.CURE;
					}
					break;
				}
			}
			else
			{
				if (ballId == 64)
				{
					return BombType.CURE;
				}
				switch (ballId)
				{
				case 97:
				case 98:
					return BombType.CURE;

				case 99:
					break;

				default:
					if (ballId != 10009)
					{
						return BombType.Normal;
					}
					return BombType.CURE;
				}
			}
			return BombType.FORZEN;
		}
	}
}
