using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BetaUpdateApp
{
    class Program
    {
        static void Main(string[] args)
        {
            KillProc("Interface_Reception_Ribbon");
            string crtPath = Environment.CurrentDirectory;
            RegistryKey rKey = Registry.CurrentUser.OpenSubKey("Environment");
            string odPath = rKey.GetValue("OneDrive") as string;
            string filePath = odPath + "\\C#\\Hostel Reception Operation System" +
                "\\Interface_Reception_Ribbon\\bin\\x86\\Debug";
            DirectoryInfo root = new DirectoryInfo(filePath);
            foreach (FileInfo fi in root.GetFiles())
            {
                if (fi.Name != "Update.exe" && (fi.Extension == ".dll" || fi.Extension == ".exe"))
                {
                    string nfp = crtPath + "\\" + fi.Name;
                    if (File.Exists(nfp))
                        File.Delete(nfp);
                    File.Copy(fi.FullName,
                        nfp);
                    Console.WriteLine("已复制文件：" + fi.Name);
                }
            }
            string usPath = odPath + "\\C#\\Hostel Reception Operation System\\update.ini";
            if (File.Exists(usPath))
            {
                FileStream fs = new FileStream(usPath, FileMode.Open);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write("IsNeedUpdate=0");
                sw.Close();
                fs.Close();
            }
            Console.WriteLine("更新完毕。按Enter退出 . . .");
            Console.ReadLine();
            string hrosPath = crtPath + "//Interface_Reception_Ribbon.exe";
            if (File.Exists(hrosPath))
            {
                Process p = new Process();
                p.StartInfo.FileName = hrosPath;
                p.StartInfo.WorkingDirectory = crtPath;
                p.Start();
            }
            else
            {
                Console.WriteLine("主程序路径错误！请再次更新或重新安装。\r\n按Enter退出 . . .");
                Console.ReadLine();
            }
        }
        /// 根据“精确进程名”结束进程
        /// </summary>
        /// <param name="strProcName">精确进程名</param>
        static void KillProc(string strProcName)
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
    }
}
