using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ReduceStrengthEffect : AbstractEffect
	{
		private int m_count;
		private int m_reduce;
		public ReduceStrengthEffect(int count, int reduce) : base(eEffectType.ReduceStrengthEffect)
		{
			this.m_count = count;
			this.m_reduce = reduce;
		}
		public override bool Start(Living living)
		{
			ReduceStrengthEffect reduceStrengthEffect = living.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) as ReduceStrengthEffect;
			if (reduceStrengthEffect != null)
			{
				reduceStrengthEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, BuffType.Tired, true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, BuffType.Tired, false);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_count--;
			if (living is Player)
			{
				(living as Player).Energy -= this.m_reduce;
			}
			if (this.m_count < 0)
			{
				this.Stop();
			}
		}
	}
}
