using Bussiness;
using Bussiness.Managers;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using Game.Logic.Spells;
using System;
namespace Game.Logic.Effects
{
	public class IceFronzeEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public IceFronzeEquipEffect(int count, int probability) : base(eEffectType.IceFronzeEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			IceFronzeEquipEffect iceFronzeEquipEffect = living.EffectList.GetOfType(eEffectType.IceFronzeEquipEffect) as IceFronzeEquipEffect;
			if (iceFronzeEquipEffect != null)
			{
				iceFronzeEquipEffect.m_probability = ((this.m_probability > iceFronzeEquipEffect.m_probability) ? this.m_probability : iceFronzeEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
		}
		private void ChangeProperty(Player player)
		{
			if (player.CurrentBall.IsSpecial())
			{
				return;
			}
			if (this.rand.Next(100) < this.m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				SpellMgr.ExecuteSpell(player.Game, player, ItemMgr.FindItemTemplate(BuffType.IceFronze));
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("IceFronzeEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
