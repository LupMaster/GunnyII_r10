using System;

namespace Game.Base
{
    public class ConsoleClient : BaseClient
    {
        public ConsoleClient()
            : base((byte[])null, (byte[])null)
        {
        }

        public override void DisplayMessage(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
