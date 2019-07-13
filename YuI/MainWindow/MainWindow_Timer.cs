using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MMC = MementoConnection.MMConnection;

namespace YuI
{
    public partial class MainWindow
    {
        static System.Threading.Timer _SuicideTimer = new System.Threading.Timer(
            new System.Threading.TimerCallback(_Suicide), null, 0, 10000);

        private static void _Suicide(object state)
        {
            if (App.IsSuicidable())
            {
                MainWindow.Pop("已切换至里世界，零点后返回。");
                Environment.Exit(0);
            }
        }

    }
}
