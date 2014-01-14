using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class SealEffect : AbstractEffect
	{
		private int m_count;
		public SealEffect(int count) : base(eEffectType.SealEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			SealEffect sealEffect = living.EffectList.GetOfType(eEffectType.SealEffect) as SealEffect;
			if (sealEffect != null)
			{
				sealEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.SetSeal(true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.SetSeal(false);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_count--;
			if (this.m_count <= 0)
			{
				this.Stop();
			}
		}
	}
}
