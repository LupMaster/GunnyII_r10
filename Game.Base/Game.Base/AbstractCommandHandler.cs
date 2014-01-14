namespace Game.Base
{
    public abstract class AbstractCommandHandler
    {
        public virtual void DisplayMessage(BaseClient client, string format, params object[] args)
        {
            this.DisplayMessage(client, string.Format(format, args));
        }

        public virtual void DisplayMessage(BaseClient client, string message)
        {
            if (client == null)
                return;
            client.DisplayMessage(message);
        }

        public virtual void DisplaySyntax(BaseClient client)
        {
            if (client == null)
                return;
            CmdAttribute[] cmdAttributeArray = (CmdAttribute[])this.GetType().GetCustomAttributes(typeof(CmdAttribute), false);
            if (cmdAttributeArray.Length > 0)
            {
                client.DisplayMessage(cmdAttributeArray[0].Description);
                foreach (string msg in cmdAttributeArray[0].Usage)
                    client.DisplayMessage(msg);
            }
        }
    }
}
