using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddTurnEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private int m_templateID;
		public AddTurnEquipEffect(int count, int probability, int templateID) : base(eEffectType.AddTurnEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_templateID = templateID;
		}
		public override bool Start(Living living)
		{
			AddTurnEquipEffect addTurnEquipEffect = living.EffectList.GetOfType(eEffectType.AddTurnEquipEffect) as AddTurnEquipEffect;
			if (addTurnEquipEffect != null)
			{
				addTurnEquipEffect.m_probability = ((this.m_probability > addTurnEquipEffect.m_probability) ? this.m_probability : addTurnEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
			player.BeginNextTurn += new LivingEventHandle(this.player_BeginSelfTurn);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
			player.BeginNextTurn -= new LivingEventHandle(this.player_BeginSelfTurn);
		}
		public void player_BeginSelfTurn(Living living)
		{
			if (this.IsTrigger)
			{
				int energy = 0;
				int templateID = this.m_templateID;
				if (templateID <= 311312)
				{
					if (templateID <= 311129)
					{
						if (templateID != 311112)
						{
							if (templateID == 311129)
							{
								energy = 145;
							}
						}
						else
						{
							energy = 130;
						}
					}
					else
					{
						if (templateID != 311212)
						{
							if (templateID != 311229)
							{
								if (templateID == 311312)
								{
									energy = 190;
								}
							}
							else
							{
								energy = 175;
							}
						}
						else
						{
							energy = 160;
						}
					}
				}
				else
				{
					if (templateID <= 311412)
					{
						if (templateID != 311329)
						{
							if (templateID == 311412)
							{
								energy = 220;
							}
						}
						else
						{
							energy = 205;
						}
					}
					else
					{
						if (templateID != 311429)
						{
							if (templateID != 311512)
							{
								if (templateID == 311529)
								{
									energy = 265;
								}
							}
							else
							{
								energy = 260;
							}
						}
						else
						{
							energy = 245;
						}
					}
				}
				(living as Player).Delay = (living as Player).Delay + (living as Player).DefaultDelay * this.m_count / 100;
				(living as Player).Energy = energy;
				this.IsTrigger = false;
			}
		}
		private void ChangeProperty(Player player)
		{
			if (player.CurrentBall.IsSpecial())
			{
				return;
			}
			if (this.rand.Next(100) < this.m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				player.Delay = player.DefaultDelay;
				this.IsTrigger = true;
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddTurnEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
