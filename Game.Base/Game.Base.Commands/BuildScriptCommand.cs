using Game.Base;
using Game.Server.Managers;

namespace Game.Base.Commands
{
    [Cmd("&cs", ePrivLevel.Player, "Compile the C# scripts.", new string[] { "/cs  <source file> <target> <importlib>", "eg: /cs ./scripts temp.dll game.base.dll,game.logic.dll" })]
    public class BuildScriptCommand : AbstractCommandHandler, ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length >= 4)
                ScriptMgr.CompileScripts(0 != 0, args[1], args[2], args[3].Split(new char[1]
        {
          ','
        }));
            else
                this.DisplaySyntax(client);
            return true;
        }
    }
}
