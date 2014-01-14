using log4net;
using System;
using System.Reflection;
using System.Text;

namespace Game.Base
{
    public class WeakMulticastDelegate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private WeakReference weakRef;
        private MethodInfo method;
        private WeakMulticastDelegate prev;

        static WeakMulticastDelegate()
        {
        }

        public WeakMulticastDelegate(Delegate realDelegate)
        {
            if (realDelegate.Target != null)
                this.weakRef = (WeakReference)new WeakRef(realDelegate.Target);
            this.method = realDelegate.Method;
        }

        public static WeakMulticastDelegate operator +(WeakMulticastDelegate d, Delegate realD)
        {
            return WeakMulticastDelegate.Combine(d, realD);
        }

        public static WeakMulticastDelegate operator -(WeakMulticastDelegate d, Delegate realD)
        {
            return WeakMulticastDelegate.Remove(d, realD);
        }

        public static WeakMulticastDelegate Combine(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
        {
            if (realDelegate == null)
                return (WeakMulticastDelegate)null;
            if (weakDelegate != null)
                return weakDelegate.Combine(realDelegate);
            else
                return new WeakMulticastDelegate(realDelegate);
        }

        public static WeakMulticastDelegate CombineUnique(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
        {
            if (realDelegate == null)
                return (WeakMulticastDelegate)null;
            if (weakDelegate != null)
                return weakDelegate.CombineUnique(realDelegate);
            else
                return new WeakMulticastDelegate(realDelegate);
        }

        private WeakMulticastDelegate Combine(Delegate realDelegate)
        {
            this.prev = new WeakMulticastDelegate(realDelegate)
            {
                prev = this.prev
            };
            return this;
        }

        protected bool Equals(Delegate realDelegate)
        {
            if (this.weakRef == null)
                return realDelegate.Target == null && this.method == realDelegate.Method;
            else
                return this.weakRef.Target == realDelegate.Target && this.method == realDelegate.Method;
        }

        private WeakMulticastDelegate CombineUnique(Delegate realDelegate)
        {
            bool flag = this.Equals(realDelegate);
            if (!flag && this.prev != null)
            {
                for (WeakMulticastDelegate multicastDelegate = this.prev; !flag && multicastDelegate != null; multicastDelegate = multicastDelegate.prev)
                {
                    if (multicastDelegate.Equals(realDelegate))
                        flag = true;
                }
            }
            if (!flag)
                return this.Combine(realDelegate);
            else
                return this;
        }

        public static WeakMulticastDelegate Remove(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
        {
            if (realDelegate == null || weakDelegate == null)
                return (WeakMulticastDelegate)null;
            else
                return weakDelegate.Remove(realDelegate);
        }

        private WeakMulticastDelegate Remove(Delegate realDelegate)
        {
            if (this.Equals(realDelegate))
                return this.prev;
            WeakMulticastDelegate multicastDelegate1 = this.prev;
            WeakMulticastDelegate multicastDelegate2 = this;
            for (; multicastDelegate1 != null; multicastDelegate1 = multicastDelegate1.prev)
            {
                if (multicastDelegate1.Equals(realDelegate))
                {
                    multicastDelegate2.prev = multicastDelegate1.prev;
                    multicastDelegate1.prev = (WeakMulticastDelegate)null;
                    break;
                }
                else
                    multicastDelegate2 = multicastDelegate1;
            }
            return this;
        }

        public void Invoke(object[] args)
        {
            for (WeakMulticastDelegate multicastDelegate = this; multicastDelegate != null; multicastDelegate = multicastDelegate.prev)
            {
                int tickCount = Environment.TickCount;
                if (multicastDelegate.weakRef == null)
                    multicastDelegate.method.Invoke((object)null, args);
                else if (multicastDelegate.weakRef.IsAlive)
                    multicastDelegate.method.Invoke(multicastDelegate.weakRef.Target, args);
                if (Environment.TickCount - tickCount > 500 && WeakMulticastDelegate.log.IsWarnEnabled)
                    WeakMulticastDelegate.log.Warn((object)string.Concat(new object[4]
          {
            (object) "Invoke took ",
            (object) (Environment.TickCount - tickCount),
            (object) "ms! ",
            (object) multicastDelegate.ToString()
          }));
            }
        }

        public void InvokeSafe(object[] args)
        {
            for (WeakMulticastDelegate multicastDelegate = this; multicastDelegate != null; multicastDelegate = multicastDelegate.prev)
            {
                int tickCount = Environment.TickCount;
                try
                {
                    if (multicastDelegate.weakRef == null)
                        multicastDelegate.method.Invoke((object)null, args);
                    else if (multicastDelegate.weakRef.IsAlive)
                        multicastDelegate.method.Invoke(multicastDelegate.weakRef.Target, args);
                }
                catch (Exception ex)
                {
                    if (WeakMulticastDelegate.log.IsErrorEnabled)
                        WeakMulticastDelegate.log.Error((object)"InvokeSafe", ex);
                }
                if (Environment.TickCount - tickCount > 500 && WeakMulticastDelegate.log.IsWarnEnabled)
                    WeakMulticastDelegate.log.Warn((object)string.Concat(new object[4]
          {
            (object) "InvokeSafe took ",
            (object) (Environment.TickCount - tickCount),
            (object) "ms! ",
            (object) multicastDelegate.ToString()
          }));
            }
        }

        public string Dump()
        {
            StringBuilder stringBuilder = new StringBuilder();
            WeakMulticastDelegate multicastDelegate = this;
            int num = 0;
            for (; multicastDelegate != null; multicastDelegate = multicastDelegate.prev)
            {
                ++num;
                if (multicastDelegate.weakRef == null)
                {
                    stringBuilder.Append("\t");
                    stringBuilder.Append(num);
                    stringBuilder.Append(") ");
                    stringBuilder.Append(multicastDelegate.method.Name);
                    stringBuilder.Append(Environment.NewLine);
                }
                else if (multicastDelegate.weakRef.IsAlive)
                {
                    stringBuilder.Append("\t");
                    stringBuilder.Append(num);
                    stringBuilder.Append(") ");
                    stringBuilder.Append(multicastDelegate.weakRef.Target);
                    stringBuilder.Append(".");
                    stringBuilder.Append(multicastDelegate.method.Name);
                    stringBuilder.Append(Environment.NewLine);
                }
                else
                {
                    stringBuilder.Append("\t");
                    stringBuilder.Append(num);
                    stringBuilder.Append(") INVALID.");
                    stringBuilder.Append(multicastDelegate.method.Name);
                    stringBuilder.Append(Environment.NewLine);
                }
            }
            return ((object)stringBuilder).ToString();
        }

        public override string ToString()
        {
            Type type = (Type)null;
            if (this.method != (MethodInfo)null)
                type = this.method.DeclaringType;
            object obj = (object)null;
            if (this.weakRef != null && this.weakRef.IsAlive)
                obj = this.weakRef.Target;
            return ((object)new StringBuilder(64).Append("method: ").Append(type == (Type)null ? "(null)" : type.FullName).Append('.').Append(this.method == (MethodInfo)null ? "(null)" : this.method.Name).Append(" target: ").Append(obj == null ? "null" : obj.ToString())).ToString();
        }
    }
}
