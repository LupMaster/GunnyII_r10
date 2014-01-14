using Game.Base.Events;
using Game.Server.Managers;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Game.Base
{
    public class CommandMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Hashtable m_cmds = new Hashtable((IEqualityComparer)StringComparer.InvariantCultureIgnoreCase);
        private static string[] m_disabledarray = new string[0];

        public static string[] DisableCommands
        {
            get
            {
                return CommandMgr.m_disabledarray;
            }
            set
            {
                CommandMgr.m_disabledarray = value == null ? new string[0] : value;
            }
        }

        static CommandMgr()
        {
        }

        public static GameCommand GetCommand(string cmd)
        {
            return CommandMgr.m_cmds[(object)cmd] as GameCommand;
        }

        public static GameCommand GuessCommand(string cmd)
        {
            GameCommand gameCommand1 = CommandMgr.GetCommand(cmd);
            if (gameCommand1 != null)
                return gameCommand1;
            string str1 = cmd.ToLower();
            IDictionaryEnumerator enumerator = CommandMgr.m_cmds.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GameCommand gameCommand2 = enumerator.Value as GameCommand;
                string str2 = enumerator.Key as string;
                if (gameCommand2 != null && str2.ToLower().StartsWith(str1))
                {
                    gameCommand1 = gameCommand2;
                    break;
                }
            }
            return gameCommand1;
        }

        public static string[] GetCommandList(ePrivLevel plvl, bool addDesc)
        {
            IDictionaryEnumerator enumerator = CommandMgr.m_cmds.GetEnumerator();
            ArrayList arrayList = new ArrayList();
            while (enumerator.MoveNext())
            {
                GameCommand gameCommand = enumerator.Value as GameCommand;
                string str = enumerator.Key as string;
                if (gameCommand != null && str != null)
                {
                    if ((int)str[0] == 38)
                        str = (string)(object)'/' + (object)str.Remove(0, 1);
                    if (plvl >= (ePrivLevel)gameCommand.m_lvl)
                    {
                        if (addDesc)
                            arrayList.Add((object)(str + " - " + gameCommand.m_desc));
                        else
                            arrayList.Add((object)gameCommand.m_cmd);
                    }
                }
            }
            return (string[])arrayList.ToArray(typeof(string));
        }

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            CommandMgr.LoadCommands();
        }

        public static bool LoadCommands()
        {
            CommandMgr.m_cmds.Clear();
            foreach (Assembly assembly in new ArrayList((ICollection)ScriptMgr.Scripts))
            {
                if (CommandMgr.log.IsDebugEnabled)
                    CommandMgr.log.Debug((object)("ScriptMgr: Searching for commands in " + (object)assembly.GetName()));
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && type.GetInterface("Game.Base.ICommandHandler") != (Type)null)
                    {
                        try
                        {
                            foreach (CmdAttribute cmdAttribute in type.GetCustomAttributes(typeof(CmdAttribute), false))
                            {
                                bool flag = false;
                                foreach (string str in CommandMgr.m_disabledarray)
                                {
                                    if (cmdAttribute.Cmd.Replace('&', '/') == str)
                                    {
                                        flag = true;
                                        CommandMgr.log.Info((object)("Will not load command " + cmdAttribute.Cmd + " as it is disabled in game properties"));
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    if (CommandMgr.m_cmds.ContainsKey((object)cmdAttribute.Cmd))
                                    {
                                        CommandMgr.log.Info((object)string.Concat(new object[4]
                    {
                      (object) cmdAttribute.Cmd,
                      (object) " from ",
                      (object) assembly.GetName(),
                      (object) " has been suppressed, a command of that type already exists!"
                    }));
                                    }
                                    else
                                    {
                                        if (CommandMgr.log.IsDebugEnabled)
                                            CommandMgr.log.Debug((object)("Load: " + cmdAttribute.Cmd + "," + cmdAttribute.Description));
                                        GameCommand gameCommand = new GameCommand();
                                        gameCommand.m_usage = cmdAttribute.Usage;
                                        gameCommand.m_cmd = cmdAttribute.Cmd;
                                        gameCommand.m_lvl = cmdAttribute.Level;
                                        gameCommand.m_desc = cmdAttribute.Description;
                                        gameCommand.m_cmdHandler = (ICommandHandler)Activator.CreateInstance(type);
                                        CommandMgr.m_cmds.Add((object)cmdAttribute.Cmd, (object)gameCommand);
                                        if (cmdAttribute.Aliases != null)
                                        {
                                            foreach (string str in cmdAttribute.Aliases)
                                                CommandMgr.m_cmds.Add((object)str, (object)gameCommand);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (CommandMgr.log.IsErrorEnabled)
                                CommandMgr.log.Error((object)"LoadCommands", ex);
                        }
                    }
                }
            }
            CommandMgr.log.Info((object)("Loaded " + (object)CommandMgr.m_cmds.Count + " commands!"));
            return true;
        }

        public static void DisplaySyntax(BaseClient client)
        {
            client.DisplayMessage("Commands list:");
            foreach (string str in CommandMgr.GetCommandList(ePrivLevel.Admin, true))
                client.DisplayMessage("         " + str);
        }

        public static bool HandleCommandNoPlvl(BaseClient client, string cmdLine)
        {
            try
            {
                string[] pars = CommandMgr.ParseCmdLine(cmdLine);
                GameCommand myCommand = CommandMgr.GuessCommand(pars[0]);
                if (myCommand == null)
                    return false;
                CommandMgr.ExecuteCommand(client, myCommand, pars);
            }
            catch (Exception ex)
            {
                if (CommandMgr.log.IsErrorEnabled)
                    CommandMgr.log.Error((object)"HandleCommandNoPlvl", ex);
            }
            return true;
        }

        private static bool ExecuteCommand(BaseClient client, GameCommand myCommand, string[] pars)
        {
            pars[0] = myCommand.m_cmd;
            return myCommand.m_cmdHandler.OnCommand(client, pars);
        }

        private static string[] ParseCmdLine(string cmdLine)
        {
            if (cmdLine == null)
                throw new ArgumentNullException("cmdLine");
            List<string> list = new List<string>();
            int num = 0;
            StringBuilder stringBuilder = new StringBuilder(cmdLine.Length >> 1);
            for (int index = 0; index < cmdLine.Length; ++index)
            {
                char ch = cmdLine[index];
                switch (num)
                {
                    case 0:
                        if ((int)ch != 32)
                        {
                            stringBuilder.Length = 0;
                            if ((int)ch == 34)
                            {
                                num = 2;
                            }
                            else
                            {
                                num = 1;
                                --index;
                            }
                            break;
                        }
                        else
                            break;
                    case 1:
                        if ((int)ch == 32)
                        {
                            list.Add(((object)stringBuilder).ToString());
                            num = 0;
                        }
                        stringBuilder.Append(ch);
                        break;
                    case 2:
                        if ((int)ch == 34)
                        {
                            list.Add(((object)stringBuilder).ToString());
                            num = 0;
                        }
                        stringBuilder.Append(ch);
                        break;
                }
            }
            if (num != 0)
                list.Add(((object)stringBuilder).ToString());
            string[] array = new string[list.Count];
            list.CopyTo(array);
            return array;
        }
    }
}
