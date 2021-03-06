using Game.Base;

namespace Game.Base.Commands
{
    [Cmd("&cmd", ePrivLevel.Admin, "Config the command system.", new string[] { "/cmd [option] <para1> <para2>      ", "eg: /cmd -reload           :Reload the command system.", "    /cmd -list             :Display all commands." })]
    public class CommandMgrSetupCommand : AbstractCommandHandler, ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "-reload":
                        CommandMgr.LoadCommands();
                        return true;
                    case "-list":
                        CommandMgr.DisplaySyntax(client);
                        return true;
                    default:
                        this.DisplaySyntax(client);
                        break;
                }
            }
            else
                this.DisplaySyntax(client);
            return true;
        }
    }
}
