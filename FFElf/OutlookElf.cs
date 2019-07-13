using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using dmNet;
using IOExtension;
using System.Threading;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using DMC = FFElf.DMConfigXDocument;

namespace FFElf
{
    public class OutlookElf
    {
        #region 输入焦点

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetGUIThreadInfo(uint hTreadID, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(uint hwnd, out uint lpdwProcessId);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int iLeft;
            public int iTop;
            public int iRight;
            public int iBottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        static IntPtr GetInputHwnd()
        {
            var info = new GUITHREADINFO();
            GetGUIThreadInfo(GetWindowThreadProcessId((uint)_Hwnd, out uint id), ref info);
            Console.WriteLine(info.flags);
            if (info.flags != 1) return IntPtr.Zero;
            return info.hwndCaret;
        }

        #endregion

        [DllImport("user32")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        #region 键盘操作

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private static void SwithTab(IntPtr hWnd)
        {
            uint KEYEVENTF_KEYUP = 2;
            byte VK_CONTROL = 0x11;
            //SetForegroundWindow(hWnd);
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(0x09, 0, 0, 0);
            keybd_event(0x09, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        }

        static void NextTab(IntPtr hwnd, int times = 1)
        {
            uint KEYEVENTF_KEYUP = 2;
            for(int i = 0; i < times; i++)
            {
                keybd_event(0x09, 0, 0, 0);
                keybd_event(0x09, 0, KEYEVENTF_KEYUP, 0);
            }
            Thread.Sleep(20);
        }

        #endregion

        static dmsoft _dm = new dmsoft();
        static IntPtr _Ptr = IntPtr.Zero;
        static int _Hwnd;
        static int _Height;
        static int _Width;
        public static bool IsInitialized { get; private set; } = false;

        /// <summary> 
        /// 返回值：0-失败，火狐浏览器未运行；1-初始化成功；2-失败，无法完成绑定;
        /// 3-失败，无法指定目录；
        /// </summary>
        public static int Init()
        {
            _Hwnd = _dm.FindWindow("MozillaWindowClass", "");
            _Ptr = new IntPtr(_Hwnd);
            if (_Hwnd == 0) return 0;
            if (_dm.SetPath(MementoPath.DMImagesDirectory) == 0) return 3;
            if (_dm.BindWindowEx(_Hwnd, "dx2", "windows", "windows", "", 0) == 1)
            {
                IsInitialized = true;
                _dm.GetClientSize(_Hwnd, out object wObj, out object hObj);
                _Width = GetSystemMetrics(0);
                _Height = GetSystemMetrics(1);
            }
            return IsInitialized ? 1 : 2;
        }
        
        public static bool SetConfirmEmail(OutlookEmail email)
        {
            SetForegroundWindow(_Ptr);
            /*if (EnterOutlookTab())
            {
                //EnterNewEmail();
                
            }*/
            try
            {
                PasteText(email.Address);
                NextTab(_Ptr, 3);
                PasteText(email.Theme);
                NextTab(_Ptr, 2);
                PasteText(email.Body);
                _dm.SetClipboard("");
                return true;
            }
            catch
            {
                return false;
            }
            //return false;
        }

        #region Private Method

        static bool IsOutlookTab() =>
            _dm.GetWindowTitle(_Hwnd) == "邮件 - Mrs Panda - Outlook - Mozilla Firefox";
            //_dm.HasPic(100, 200, 300, 300, "newEmail.bmp", "000000", 1.00, 0);
            
        static bool IsNewEmailPage() =>
            _dm.HasPic(0, 0, 2000, 2000, DMC.GetImageName("send"), "000000", 1.00, 0);

        static void PasteText(string text)
        {
            _dm.SetClipboard(text);
            #region  弃用
            /*IntPtr ptr = IntPtr.Zero;
            while (ptr == IntPtr.Zero)
            {
                ptr = GetInputHwnd();
                Thread.Sleep(100);
            }*/
            #endregion
            Thread.Sleep(200);
            _dm.SendPaste(_Hwnd);
            Thread.Sleep(300);
        }

        static bool EnterOutlookTab()
        {
            int times = 0;
            bool isOutlookTab = IsOutlookTab();
            while (!isOutlookTab && times < 10)
            {
                SwithTab(_Ptr);
                Thread.Sleep(500);
                isOutlookTab = IsOutlookTab();
                times++;
            }
            return isOutlookTab;
        }

        static bool EnterNewEmail()
        {
            //if (IsNewEmailPage()) return true;
            (int, int) nel = DMC.GetCoord("newEmail");
            _dm.MoveTo(nel.Item1, nel.Item2);
            _dm.LeftClick();
            Thread.Sleep(3500);
            return IsNewEmailPage();
        }

        #endregion
    }

    #region DMExtension

    public static class DMExtension
    {
        public static bool HasPic(this dmsoft dm, int x1, int y1, int x2, int y2, string pic_name,
            string delta_color, double sim, int dir)
        {
            var l = dm.FindPicLocation(x1, y1, x2, y2, pic_name, delta_color, sim, dir);
            return l.Item1 > -1 && l.Item2 > -1;
        }

        public static (int,int) FindPicLocation(this dmsoft dm, int x1, int y1, int x2, int y2, string pic_name,
            string delta_color, double sim, int dir)
        {
            dm.FindPic(x1, y1, x2, y2, pic_name, delta_color, sim, dir,
                out object x, out object y);
            return ((int)x, (int)y);
        }

    }

    #endregion
}
