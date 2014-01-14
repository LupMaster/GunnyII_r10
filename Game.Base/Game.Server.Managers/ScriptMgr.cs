using log4net;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Game.Server.Managers
{
    public class ScriptMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Dictionary<string, Assembly> m_scripts = new Dictionary<string, Assembly>();

        public static Assembly[] Scripts
        {
            get
            {
                Assembly[] assemblyArray;
                lock (ScriptMgr.m_scripts)
                    assemblyArray = Enumerable.ToArray<Assembly>((IEnumerable<Assembly>)ScriptMgr.m_scripts.Values);
                return assemblyArray;
            }
        }

        static ScriptMgr()
        {
        }

        public static bool InsertAssembly(Assembly ass)
        {
            bool flag;
            lock (ScriptMgr.m_scripts)
            {
                if (!ScriptMgr.m_scripts.ContainsKey(ass.FullName))
                {
                    ScriptMgr.m_scripts.Add(ass.FullName, ass);
                    flag = true;
                }
                else
                    flag = false;
            }
            return flag;
        }

        public static bool RemoveAssembly(Assembly ass)
        {
            bool flag;
            lock (ScriptMgr.m_scripts)
                flag = ScriptMgr.m_scripts.Remove(ass.FullName);
            return flag;
        }

        public static bool CompileScripts(bool compileVB, string path, string dllName, string[] asm_names)
        {
            if (!path.EndsWith("\\") && !path.EndsWith("/"))
                path = path + "/";
            ArrayList arrayList = ScriptMgr.ParseDirectory(new DirectoryInfo(path), compileVB ? "*.vb" : "*.cs", true);
            if (arrayList.Count == 0)
                return true;
            if (File.Exists(dllName))
                File.Delete(dllName);
            CompilerResults compilerResults = (CompilerResults)null;
            try
            {
                CodeDomProvider codeDomProvider = !compileVB ? (CodeDomProvider)new CSharpCodeProvider() : (CodeDomProvider)new VBCodeProvider();
                CompilerParameters options = new CompilerParameters(asm_names, dllName, true);
                options.GenerateExecutable = false;
                options.GenerateInMemory = false;
                options.WarningLevel = 2;
                options.CompilerOptions = "/lib:.";
                string[] strArray = new string[arrayList.Count];
                for (int index = 0; index < arrayList.Count; ++index)
                    strArray[index] = ((FileSystemInfo)arrayList[index]).FullName;
                compilerResults = codeDomProvider.CompileAssemblyFromFile(options, strArray);
                GC.Collect();
                if (compilerResults.Errors.HasErrors)
                {
                    foreach (CompilerError compilerError in (CollectionBase)compilerResults.Errors)
                    {
                        if (!compilerError.IsWarning)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.Append("   ");
                            stringBuilder.Append(compilerError.FileName);
                            stringBuilder.Append(" Line:");
                            stringBuilder.Append(compilerError.Line);
                            stringBuilder.Append(" Col:");
                            stringBuilder.Append(compilerError.Column);
                            if (ScriptMgr.log.IsErrorEnabled)
                            {
                                ScriptMgr.log.Error((object)"Script compilation failed because: ");
                                ScriptMgr.log.Error((object)compilerError.ErrorText);
                                ScriptMgr.log.Error((object)((object)stringBuilder).ToString());
                            }
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (ScriptMgr.log.IsErrorEnabled)
                    ScriptMgr.log.Error((object)"CompileScripts", ex);
            }
            if (compilerResults == null || compilerResults.Errors.HasErrors)
                return true;
            ScriptMgr.InsertAssembly(compilerResults.CompiledAssembly);
            return true;
        }

        private static ArrayList ParseDirectory(DirectoryInfo path, string filter, bool deep)
        {
            ArrayList arrayList = new ArrayList();
            if (!path.Exists)
                return arrayList;
            arrayList.AddRange((ICollection)path.GetFiles(filter));
            if (deep)
            {
                foreach (DirectoryInfo path1 in path.GetDirectories())
                    arrayList.AddRange((ICollection)ScriptMgr.ParseDirectory(path1, filter, deep));
            }
            return arrayList;
        }

        public static Type GetType(string name)
        {
            foreach (Assembly assembly in ScriptMgr.Scripts)
            {
                Type type = assembly.GetType(name);
                if (type != (Type)null)
                    return type;
            }
            return (Type)null;
        }

        public static object CreateInstance(string name)
        {
            foreach (Assembly assembly in ScriptMgr.Scripts)
            {
                Type type = assembly.GetType(name);
                if (type != (Type)null && type.IsClass)
                    return Activator.CreateInstance(type);
            }
            return (object)null;
        }

        public static object CreateInstance(string name, Type baseType)
        {
            foreach (Assembly assembly in ScriptMgr.Scripts)
            {
                Type type = assembly.GetType(name);
                if (type != (Type)null && type.IsClass && baseType.IsAssignableFrom(type))
                    return Activator.CreateInstance(type);
            }
            return (object)null;
        }

        public static Type[] GetDerivedClasses(Type baseType)
        {
            if (baseType == (Type)null)
                return new Type[0];
            ArrayList arrayList = new ArrayList();
            foreach (Assembly assembly in new ArrayList((ICollection)ScriptMgr.Scripts))
            {
                foreach (Type c in assembly.GetTypes())
                {
                    if (c.IsClass && baseType.IsAssignableFrom(c))
                        arrayList.Add((object)c);
                }
            }
            return (Type[])arrayList.ToArray(typeof(Type));
        }

        public static Type[] GetImplementedClasses(string baseInterface)
        {
            ArrayList arrayList = new ArrayList();
            foreach (Assembly assembly in new ArrayList((ICollection)ScriptMgr.Scripts))
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && type.GetInterface(baseInterface) != (Type)null)
                        arrayList.Add((object)type);
                }
            }
            return (Type[])arrayList.ToArray(typeof(Type));
        }
    }
}
