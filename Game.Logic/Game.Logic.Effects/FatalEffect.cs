using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class FatalEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private int m_saycount;
		public FatalEffect(int count, int probability) : base(eEffectType.FatalEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			FatalEffect fatalEffect = living.EffectList.GetOfType(eEffectType.FatalEffect) as FatalEffect;
			if (fatalEffect != null)
			{
				fatalEffect.m_probability = ((this.m_probability > fatalEffect.m_probability) ? this.m_probability : fatalEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
			player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.BeginNextTurn += new LivingEventHandle(this.player_BeginNextTurn);
			player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
			player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.BeginNextTurn -= new LivingEventHandle(this.player_BeginNextTurn);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_AfterPlayerShooted(Player player)
		{
			this.IsTrigger = false;
			player.ControlBall = false;
			player.EffectTrigger = false;
		}
		private void player_BeginNextTurn(Living living)
		{
			this.m_saycount = 0;
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
		{
			if (this.IsTrigger || living is Player)
			{
				damageAmount = damageAmount * (100 - this.m_count) / 100;
			}
		}
		private void ChangeProperty(Player player)
		{
			if (player.CurrentBall.IsSpecial())
			{
				return;
			}
			this.m_saycount++;
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				player.ShootMovieDelay = 50;
				this.IsTrigger = true;
				if (player.CurrentBall.ID != 3)
				{
					player.ControlBall = true;
				}
				if (this.m_saycount == 1)
				{
					player.EffectTrigger = true;
					player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
					player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("FatalEffect.msg", new object[0]), 9, 0, 1000));
				}
			}
		}
	}
}
