using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddBombEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private bool m_show;
		public AddBombEquipEffect(int count, int probability) : base(eEffectType.AddBombEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddBombEquipEffect addBombEquipEffect = living.EffectList.GetOfType(eEffectType.AddBombEquipEffect) as AddBombEquipEffect;
			if (addBombEquipEffect != null)
			{
				this.m_probability = ((this.m_probability > addBombEquipEffect.m_probability) ? this.m_probability : addBombEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerShoot += new PlayerEventHandle(this.playerShot);
			player.BeginAttacking += new LivingEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerShoot -= new PlayerEventHandle(this.playerShot);
			player.BeginAttacking -= new LivingEventHandle(this.ChangeProperty);
		}
		private void playerShot(Player player)
		{
			if (this.IsTrigger && this.m_show)
			{
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddBombEquipEffect.msg", new object[0]), 9, 0, 1000));
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success", new object[0]));
				this.m_show = false;
			}
		}
		private void ChangeProperty(Living living)
		{
			if ((living as Player).CurrentBall.IsSpecial())
			{
				return;
			}
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability && living.AttackGemLimit == 0)
			{
				this.m_show = true;
				living.AttackGemLimit = 4;
				this.IsTrigger = true;
				(living as Player).ShootCount += this.m_count;
				living.EffectTrigger = true;
			}
		}
	}
}
