using BetaUpdateApp.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace BetaUpdateApp
{
    public enum UpdateMode
    {
        Developer,
        User
    }

    public partial class UMForm : Form
    {
        UpdateMode _UpdateMode;
        string _StartPath = Environment.CurrentDirectory;
        string _OneDrivePath;
        string _DataPath;
        string _FPath;
        string _TPath;
        bool _Updated = false;

        public UMForm()
        {
            InitializeComponent();
        }

        private void UMForm_Load(object sender, EventArgs e)
        {
            //_StartPath = "C:\\Program Files (x86)\\E.A\\YuI";
            _UpdateMode = _StartPath.Contains("Debug") ? UpdateMode.Developer : UpdateMode.User;
            RegistryKey rKey = Registry.CurrentUser.OpenSubKey("Environment");
            _OneDrivePath = rKey.GetValue("OneDrive") as string + "\\Update for " + Resources.MainAppName;
            _DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                "\\HROS";
            if (_UpdateMode == UpdateMode.Developer)
            {
                _FPath = _StartPath;
                _TPath = _OneDrivePath;
                int index = _FPath.IndexOf("BetaUpdateApp");
                if (index > -1)
                    _FPath = _FPath.Substring(0, index) + "\\Interface_Reception_Ribbon\\" +
                        "bin\\x86\\Debug";
            }
            else
            {
                _FPath = _OneDrivePath;
                _TPath = _StartPath;
            }
            if (!Directory.Exists(_StartPath))
            {
                MessageBox.Show("默认更新目录不存在！退出更新！");
                this.Close();
            }
        }

        /// 根据“精确进程名”结束进程
        /// </summary>
        /// <param name="strProcName">精确进程名</param>
        public static void KillProc(string strProcName)
        {
            Console.WriteLine("正在尝试结束进程：" + strProcName);
            try
            {
                Process[] processes = Process.GetProcessesByName(strProcName);
                while (processes.Length > 0)
                {
                    foreach (Process p in processes)
                    {
                        if (!p.CloseMainWindow())
                            p.Kill();
                    }
                    Thread.Sleep(200);
                    processes = Process.GetProcessesByName(strProcName);
                }
            }
            catch
            {

            }
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            _Updated = true;
            KillProc(Resources.MainAppName);
            bool isAppOk = cB_app.Checked && UpdateMainApp();
            bool isDBOk = cB_db.Checked && UpdateDataBase();
            bool isCfOk = cB_config.Checked && UpdateConfig();
            MessageBox.Show(string.Format(
                "更新结果：\r\n程序主体={0}\r\n数据库文件={1}\r\n配置文件={2}",
                isAppOk ? "成功" : "失败",
                isDBOk ? "成功" : "失败",
                isCfOk ? "成功" : "失败"));
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RunMainApp()
        {
            string hrosPath = _StartPath + "\\" + Resources.MainAppName + ".exe";
            if (File.Exists(hrosPath))
            {
                Process p = new Process();
                p.StartInfo.FileName = hrosPath;
                p.StartInfo.WorkingDirectory = _StartPath;
                p.Start();
            }
            else
            {
                MessageBox.Show("主程序路径错误！请再次更新或重新安装 . . .");
            }
        }

        private bool UpdateMainApp()
        {
            List<string> fileList = GetAppFileList();
            fileList.Add(Resources.MainAppName + ".exe");
            if (!Directory.Exists(_TPath))
                Directory.CreateDirectory(_TPath);
            try
            {
                fileList.ForEach(file =>
                {
                    File.Copy(_FPath + "\\" + file, _TPath + "\\" + file, true);
                });
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool UpdateDataBase()
        {
            try
            {
                string filePathF = _OneDrivePath + "\\Data\\database\\" + Resources.DataBaseFileName;
                string filePathT = _DataPath + "\\database\\" + Resources.DataBaseFileName;
                if (_UpdateMode == UpdateMode.Developer)
                {
                    string tmp = filePathF;
                    filePathF = filePathT;
                    filePathT = tmp;
                }
                if (File.Exists(filePathT)) File.Delete(filePathT);
                File.Copy(filePathF, filePathT);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool UpdateConfig()
        {
            try
            {
                string filePathF = _OneDrivePath + "\\Data\\" + Resources.ResConfigFileName;
                string filePathT = _DataPath + "\\" + Resources.ResConfigFileName;
                if (_UpdateMode == UpdateMode.Developer)
                {
                    string tmp = filePathF;
                    filePathF = filePathT;
                    filePathT = tmp;
                }
                if (File.Exists(filePathT)) File.Delete(filePathT);
                File.Copy(filePathF, filePathT);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<string> GetAppFileList()
        {
            List<string> files = new List<string>();
            Assembly _assembly = Assembly.GetExecutingAssembly();
            Stream fs = _assembly.GetManifestResourceStream(
                "BetaUpdateApp.Resources.appFileList.txt");
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                files.Add(sr.ReadLine());
            }
            sr.Close();
            fs.Close();
            return files;
        }

        private void UMForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_Updated && _UpdateMode == UpdateMode.User)
                RunMainApp();
        }
    }
}
