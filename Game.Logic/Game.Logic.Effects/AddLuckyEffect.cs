using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddLuckyEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private int m_added;
		public AddLuckyEffect(int count, int probability) : base(eEffectType.AddLuckyEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_added = 0;
		}
		public override bool Start(Living living)
		{
			AddLuckyEffect addLuckyEffect = living.EffectList.GetOfType(eEffectType.AddLuckyEffect) as AddLuckyEffect;
			if (addLuckyEffect != null)
			{
				addLuckyEffect.m_probability = ((this.m_probability > addLuckyEffect.m_probability) ? this.m_probability : addLuckyEffect.m_probability);
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
			player.Lucky -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				player.FlyingPartical = 65;
				this.IsTrigger = true;
				player.Lucky += (double)this.m_count;
				player.EffectTrigger = true;
				this.m_added = this.m_count;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddLuckyEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
