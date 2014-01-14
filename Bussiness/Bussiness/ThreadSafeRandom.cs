using System;

namespace Bussiness
{
    public class ThreadSafeRandom
    {
        private static Random randomStatic = new Random();
        private Random random = new Random();

        static ThreadSafeRandom()
        {
        }

        public static int NextStatic()
        {
            int num;
            lock (ThreadSafeRandom.randomStatic)
                num = ThreadSafeRandom.randomStatic.Next();
            return num;
        }

        public static int NextStatic(int maxValue)
        {
            int num;
            lock (ThreadSafeRandom.randomStatic)
                num = ThreadSafeRandom.randomStatic.Next(maxValue);
            return num;
        }

        public static int NextStatic(int minValue, int maxValue)
        {
            int num;
            lock (ThreadSafeRandom.randomStatic)
                num = ThreadSafeRandom.randomStatic.Next(minValue, maxValue);
            return num;
        }

        public static void NextStatic(byte[] keys)
        {
            lock (ThreadSafeRandom.randomStatic)
                ThreadSafeRandom.randomStatic.NextBytes(keys);
        }

        public int Next()
        {
            int num;
            lock (this.random)
                num = this.random.Next();
            return num;
        }

        public int Next(int maxValue)
        {
            int num;
            lock (this.random)
                num = this.random.Next(maxValue);
            return num;
        }

        public int Next(int minValue, int maxValue)
        {
            int num;
            lock (this.random)
                num = this.random.Next(minValue, maxValue);
            return num;
        }
    }
}
