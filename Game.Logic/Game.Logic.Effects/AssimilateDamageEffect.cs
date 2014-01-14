using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AssimilateDamageEffect : BasePlayerEffect
	{
		private int m_type;
		private int m_count;
		private int m_probability;
		public AssimilateDamageEffect(int count, int probability, int type) : base(eEffectType.AssimilateDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_type = type;
		}
		public override bool Start(Living living)
		{
			AssimilateDamageEffect assimilateDamageEffect = living.EffectList.GetOfType(eEffectType.AssimilateDamageEffect) as AssimilateDamageEffect;
			if (assimilateDamageEffect != null)
			{
				assimilateDamageEffect.m_probability = ((this.m_probability > assimilateDamageEffect.m_probability) ? this.m_probability : assimilateDamageEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability && living.DefendActiveGem == this.m_type)
			{
				this.IsTrigger = true;
				living.EffectTrigger = true;
				living.SyncAtTime = true;
				if (damageAmount > this.m_count)
				{
					living.AddBlood(this.m_count);
				}
				else
				{
					living.AddBlood(damageAmount);
				}
				living.SyncAtTime = false;
				damageAmount -= damageAmount;
				criticalAmount -= criticalAmount;
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AssimilateDamageEffect.msg", new object[0]), 9, 0, 1000));
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success", new object[0]));
			}
		}
	}
}
