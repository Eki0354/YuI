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
        public static string SavedPasswordFilePath = Environment.CurrentDirectory + @"\sp.db";
        public static string BackupDBToODDirectory = Environment.CurrentDirectory + @"\backup";
        public static string BackupDBToODFilePath = BackupDBToODDirectory + @"\mm.db";
        public static string BackupRCToODFilePath = BackupDBToODDirectory + @"\rc.xml";
    }
}
