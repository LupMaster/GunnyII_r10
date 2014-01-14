using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class NoHoleEquipEffect : BasePlayerEffect
	{
		private int m_type;
		private int m_count;
		private int m_probability;
		public NoHoleEquipEffect(int count, int probability, int type) : base(eEffectType.NoHoleEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_type = type;
		}
		public override bool Start(Living living)
		{
			NoHoleEquipEffect noHoleEquipEffect = living.EffectList.GetOfType(eEffectType.NoHoleEquipEffect) as NoHoleEquipEffect;
			if (noHoleEquipEffect != null)
			{
				noHoleEquipEffect.m_probability = ((this.m_probability > noHoleEquipEffect.m_probability) ? this.m_probability : noHoleEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.CollidByObject += new PlayerEventHandle(this.player_AfterKilledByLiving);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.CollidByObject -= new PlayerEventHandle(this.player_AfterKilledByLiving);
		}
		private void player_AfterKilledByLiving(Living living)
		{
			if (this.rand.Next(100) < this.m_probability && living.DefendActiveGem == this.m_type)
			{
				living.EffectTrigger = true;
				new NoHoleEffect(1).Start(living);
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success", new object[0]));
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("NoHoleEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
