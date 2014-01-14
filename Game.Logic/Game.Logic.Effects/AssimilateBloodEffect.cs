using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AssimilateBloodEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AssimilateBloodEffect(int count, int probability) : base(eEffectType.AssimilateBloodEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AssimilateBloodEffect assimilateBloodEffect = living.EffectList.GetOfType(eEffectType.AssimilateBloodEffect) as AssimilateBloodEffect;
			if (assimilateBloodEffect != null)
			{
				assimilateBloodEffect.m_probability = ((this.m_probability > assimilateBloodEffect.m_probability) ? this.m_probability : assimilateBloodEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.PlayerShoot += new PlayerEventHandle(this.player_PlayerShoot);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.PlayerShoot -= new PlayerEventHandle(this.player_PlayerShoot);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
		{
			if (living.IsLiving || this.IsTrigger)
			{
				living.SyncAtTime = true;
				living.AddBlood(damageAmount * this.m_count / 100);
				living.SyncAtTime = false;
			}
		}
		private void player_PlayerShoot(Player player)
		{
			if (player.CurrentBall.IsSpecial())
			{
				return;
			}
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				this.IsTrigger = true;
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AssimilateBloodEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
