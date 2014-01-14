using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddDanderEquipEffect : BasePlayerEffect
	{
		private int m_type;
		private int m_count;
		private int m_probability;
		public AddDanderEquipEffect(int count, int probability, int type) : base(eEffectType.AddDander)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_type = type;
		}
		public override bool Start(Living living)
		{
			AddDanderEquipEffect addDanderEquipEffect = living.EffectList.GetOfType(eEffectType.AddDander) as AddDanderEquipEffect;
			if (addDanderEquipEffect != null)
			{
				this.m_probability = ((this.m_probability > addDanderEquipEffect.m_probability) ? this.m_probability : addDanderEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeginAttacked += new LivingEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeginAttacked -= new LivingEventHandle(this.ChangeProperty);
		}
		private void ChangeProperty(Living living)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability && living.DefendActiveGem == this.m_type)
			{
				this.IsTrigger = true;
				if (living is Player)
				{
					(living as Player).AddDander(this.m_count);
				}
				living.EffectTrigger = true;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success", new object[0]));
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AddDanderEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
