using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.PetEffects
{
	public abstract class AbstractPetEffect
	{
		private ePetEffectType m_type;
		protected Living m_living;
		protected Random rand;
		public bool IsTrigger;
		public ePetEffectType Type
		{
			get
			{
				return this.m_type;
			}
		}
		public int TypeValue
		{
			get
			{
				return (int)this.m_type;
			}
		}
		public AbstractPetEffect(ePetEffectType type)
		{
			this.rand = new Random();
			this.m_type = type;
		}
		public virtual bool Start(Living living)
		{
			this.m_living = living;
			return this.m_living.PetEffectList.Add(this);
		}
		public virtual bool Stop()
		{
			return this.m_living != null && this.m_living.PetEffectList.Remove(this);
		}
		public virtual void OnAttached(Living living)
		{
		}
		public virtual void OnRemoved(Living living)
		{
		}
	}
}
