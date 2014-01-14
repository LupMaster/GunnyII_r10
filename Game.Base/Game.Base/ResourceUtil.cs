using System.IO;
using System.Reflection;

namespace Game.Base
{
    public class ResourceUtil
    {
        public static Stream GetResourceStream(string fileName, Assembly assem)
        {
            fileName = fileName.ToLower();
            foreach (string name in assem.GetManifestResourceNames())
            {
                if (name.ToLower().EndsWith(fileName))
                    return assem.GetManifestResourceStream(name);
            }
            return (Stream)null;
        }

        public static void ExtractResource(string fileName, Assembly assembly)
        {
            ResourceUtil.ExtractResource(fileName, fileName, assembly);
        }

        public static void ExtractResource(string resourceName, string fileName, Assembly assembly)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            using (StreamReader streamReader = new StreamReader(ResourceUtil.GetResourceStream(resourceName, assembly)))
            {
                using (StreamWriter streamWriter = new StreamWriter((Stream)File.Create(fileName)))
                    streamWriter.Write(streamReader.ReadToEnd());
            }
        }
    }
}
