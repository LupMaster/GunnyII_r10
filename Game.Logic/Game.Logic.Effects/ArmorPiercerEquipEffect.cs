using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ArmorPiercerEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public ArmorPiercerEquipEffect(int count, int probability) : base(eEffectType.ArmorPiercer)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			ArmorPiercerEquipEffect armorPiercerEquipEffect = living.EffectList.GetOfType(eEffectType.ArmorPiercer) as ArmorPiercerEquipEffect;
			if (armorPiercerEquipEffect != null)
			{
				armorPiercerEquipEffect.m_probability = ((this.m_probability > armorPiercerEquipEffect.m_probability) ? this.m_probability : armorPiercerEquipEffect.m_probability);
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
			player.IgnoreArmor = false;
			if (this.rand.Next(100) < this.m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				player.FlyingPartical = 65;
				player.IgnoreArmor = true;
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("ArmorPiercerEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
