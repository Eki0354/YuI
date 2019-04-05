using System;

namespace IOExtension
{
    public class MementoPath
    {
        public static string AppDataDirectory =>
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YuI";
        public static string EmailTemplatesDirectory = AppDataDirectory + "\\EmailTemplates";
        public static string DataBaseDirectory => AppDataDirectory + "\\database";
        public static string MainDataBasePath => DataBaseDirectory + "\\mm.db";
        public static string ResConfigPath => AppDataDirectory + "\\ResConfig.xml";
    }

    public static class PathExtension
    {
        public static string GetParentDirectory(this string path)
        {
            try
            {
                return path.Substring(0, path.LastIndexOf(@"\"));
            }
            catch
            {
                return null;
            }
        }
    }
}
