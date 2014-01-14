using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class FightAchievementAction : BaseAction
	{
		private Living m_living;
		private int m_num;
		private int m_type;
		private int m_delay;
		public FightAchievementAction(Living living, int type, int num, int delay) : base(delay, 1500)
		{
			this.m_living = living;
			this.m_num = num;
			this.m_type = type;
			this.m_delay = delay;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			game.SendFightAchievement(this.m_living, this.m_type, this.m_num, this.m_delay);
			base.Finish(tick);
		}
	}
}
