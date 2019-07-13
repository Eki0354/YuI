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
        public static string MainDataBasePath => DataBaseDirectory + "\\mm.db";
        public static string ResConfigPath => BackupDBToODDirectory + "\\ResConfig.xml";
        public static string RanConfigPath => BackupDBToODDirectory + "\\RanConfig.xml";
        public static string DMConfigPath => BackupDBToODDirectory + "\\DMConfig.xml";
        public static string SavedPasswordFilePath = Environment.CurrentDirectory + @"\sp.db";
        public static string BackupDBToODDirectory = Environment.CurrentDirectory + @"\backup";
        public static string BackupDBToODFilePath = BackupDBToODDirectory + @"\mm-b.db";
        public static string BackupRCToODFilePath = BackupDBToODDirectory + @"\rc.xml";
        public static string RanImagesPath => Environment.CurrentDirectory + @"\Images";
        public static string SignaturePath => BackupDBToODDirectory + "\\Sign.html";
        public static string BodyPath => BackupDBToODDirectory + "\\Body.html";

        public static string OneDriveDirectory => Registry.CurrentUser.OpenSubKey(
            "Environment").GetValue("OneDrive") as string;
        public static string OneDriveYuIDirectory => OneDriveDirectory + "\\YuI";

        public static string DMImagesDirectory => Environment.CurrentDirectory + @"\DMImages";
    }
}
