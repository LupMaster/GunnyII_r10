using System;

namespace Game.Base
{
    public class WeakRef : WeakReference
    {
        private static readonly WeakRef.NullValue NULL = new WeakRef.NullValue();

        public override object Target
        {
            get
            {
                object target = base.Target;
                if (target != WeakRef.NULL)
                    return target;
                else
                    return (object)null;
            }
            set
            {
                base.Target = value == null ? (object)WeakRef.NULL : value;
            }
        }

        static WeakRef()
        {
        }

        public WeakRef(object target)
            : base(target == null ? (object)WeakRef.NULL : target)
        {
        }

        public WeakRef(object target, bool trackResurrection)
            : base(target == null ? (object)WeakRef.NULL : target, trackResurrection)
        {
        }

        private class NullValue
        {
        }
    }
}
