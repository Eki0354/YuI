using Microsoft.Win32;
using System;

namespace IOExtension
{
    public class MementoPath
    {
        public static string AppDataDirectory =>
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YuI";
        public static string EmailTemplatesDirectory = AppDataDirectory + "\\EmailTemplates";
        public static string DataBaseDirectory => AppDataDirectory + "\\database";
        public static string MainDataBasePath => Environment.UserName.Contains("Panda") ?
            BackupDBToODFilePath : DataBaseDirectory + "\\mm.db";
        public static string ResConfigPath => BackupDBToODDirectory + "\\ResConfig.xml";
        public static string SavedPasswordFilePath = Environment.CurrentDirectory + @"\sp.db";
        public static string BackupDBToODDirectory = OneDriveYuIDirectory + @"\backup";
        public static string BackupDBToODFilePath = BackupDBToODDirectory + @"\mm.db";
        public static string BackupRCToODFilePath = BackupDBToODDirectory + @"\rc.xml";

        public static string OneDriveDirectory => Registry.CurrentUser.OpenSubKey(
            "Environment").GetValue("OneDrive") as string;
        public static string OneDriveYuIDirectory => OneDriveDirectory + "\\YuI";
    }
}
