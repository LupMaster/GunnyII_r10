using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic.Actions
{
	public class LivingRangeAttackingAction : BaseAction
	{
		private Living m_living;
		private List<Player> m_players;
		private int m_fx;
		private int m_tx;
		private string m_action;
		public LivingRangeAttackingAction(Living living, int fx, int tx, string action, int delay, List<Player> players) : base(delay, 1000)
		{
			this.m_living = living;
			this.m_players = players;
			this.m_fx = fx;
			this.m_tx = tx;
			this.m_action = action;
		}
		private int MakeDamage(Living p)
		{
			double baseDamage = this.m_living.BaseDamage;
			double num = p.BaseGuard;
			double num2 = p.Defence;
			double attack = this.m_living.Attack;
			if (p.AddArmor && (p as Player).DeputyWeapon != null)
			{
				int num3 = (p as Player).DeputyWeapon.Template.Property7 + (int)Math.Pow(1.1, (double)(p as Player).DeputyWeapon.StrengthenLevel);
				num += (double)num3;
				num2 += (double)num3;
			}
			if (this.m_living.IgnoreArmor)
			{
				num = 0.0;
				num2 = 0.0;
			}
			float currentDamagePlus = this.m_living.CurrentDamagePlus;
			float currentShootMinus = this.m_living.CurrentShootMinus;
			double num4 = 0.95 * (num - (double)(3 * this.m_living.Grade)) / (500.0 + num - (double)(3 * this.m_living.Grade));
			double num5;
			if (num2 - this.m_living.Lucky < 0.0)
			{
				num5 = 0.0;
			}
			else
			{
				num5 = 0.95 * (num2 - this.m_living.Lucky) / (600.0 + num2 - this.m_living.Lucky);
			}
			double num6 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num4 + num5 - num4 * num5)) * (double)currentDamagePlus * (double)currentShootMinus;
			Rectangle directDemageRect = p.GetDirectDemageRect();
			double num7 = Math.Sqrt((double)((directDemageRect.X - this.m_living.X) * (directDemageRect.X - this.m_living.X) + (directDemageRect.Y - this.m_living.Y) * (directDemageRect.Y - this.m_living.Y)));
			num6 *= 1.0 - num7 / (double)Math.Abs(this.m_tx - this.m_fx) / 4.0;
			if (num6 < 0.0)
			{
				return 1;
			}
			return (int)num6;
		}
		private int MakeCriticalDamage(Living p, int baseDamage)
		{
			double lucky = this.m_living.Lucky;
			Random random = new Random();
			bool flag = 75000.0 * lucky / (lucky + 800.0) > (double)random.Next(100000);
			if (flag)
			{
				int num = p.ReduceCritFisrtGem + p.ReduceCritSecondGem;
				int num2 = (int)((0.5 + lucky * 0.0003) * (double)baseDamage);
				return num2 * (100 - num) / 100;
			}
			return 0;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, this.m_living.Id);
			gSPacketIn.Parameter1 = this.m_living.Id;
			gSPacketIn.WriteByte(61);
			List<Living> list = game.Map.FindPlayers(this.m_fx, this.m_tx, this.m_players);
			int num = list.Count;
			foreach (Living current in list)
			{
				if (this.m_living.IsFriendly(current))
				{
					num--;
				}
			}
			gSPacketIn.WriteInt(num);
			this.m_living.SyncAtTime = false;
			try
			{
				foreach (Living current2 in list)
				{
					current2.SyncAtTime = false;
					if (!this.m_living.IsFriendly(current2))
					{
						int val = 0;
						current2.IsFrost = false;
						game.SendGameUpdateFrozenState(current2);
						int num2 = this.MakeDamage(current2);
						int num3 = this.MakeCriticalDamage(current2, num2);
						int val2 = 0;
						if (current2.TakeDamage(this.m_living, ref num2, ref num3, "范围攻击"))
						{
							val2 = num2 + num3;
							if (current2 is Player)
							{
								Player player = current2 as Player;
								val = player.Dander;
							}
						}
						gSPacketIn.WriteInt(current2.Id);
						gSPacketIn.WriteInt(val2);
						gSPacketIn.WriteInt(current2.Blood);
						gSPacketIn.WriteInt(val);
						gSPacketIn.WriteInt(1);
					}
				}
				game.SendToAll(gSPacketIn);
				base.Finish(tick);
			}
			finally
			{
				this.m_living.SyncAtTime = true;
				foreach (Living current3 in list)
				{
					current3.SyncAtTime = true;
				}
			}
		}
	}
}
