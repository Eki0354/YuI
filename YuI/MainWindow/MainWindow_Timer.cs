using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IOExtension;

namespace YuI
{
    public partial class MainWindow
    {
        static System.Threading.Timer TimerBackupDB = new System.Threading.Timer(
            _TimerBackupDBCallback, null, 0, 3600000);

        private static void _TimerBackupDBCallback(object state)
        {
            if (!Directory.Exists(MementoPath.BackupDBToODDirectory))
                Directory.CreateDirectory(MementoPath.BackupDBToODDirectory);
            File.Copy(MementoPath.MainDataBasePath, MementoPath.BackupDBToODFilePath, true);
            File.Copy(MementoPath.ResConfigPath, MementoPath.BackupRCToODFilePath, true);
        }
    }
}
