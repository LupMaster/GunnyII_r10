using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddAttackEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private int m_added;
		public AddAttackEffect(int count, int probability) : base(eEffectType.AddAttackEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddAttackEffect addAttackEffect = living.EffectList.GetOfType(eEffectType.AddAttackEffect) as AddAttackEffect;
			if (addAttackEffect != null)
			{
				this.m_probability = ((this.m_probability > addAttackEffect.m_probability) ? this.m_probability : addAttackEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeforePlayerShoot += new PlayerEventHandle(this.ChangeProperty);
			player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_AfterPlayerShooted(Player player)
		{
			player.FlyingPartical = 0;
		}
		private void ChangeProperty(Player player)
		{
			if (player.CurrentBall.IsSpecial())
			{
				return;
			}
			player.Attack -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				player.FlyingPartical = 65;
				this.IsTrigger = true;
				player.EffectTrigger = true;
				player.Attack += (double)this.m_count;
				this.m_added = this.m_count;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddAttackEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
