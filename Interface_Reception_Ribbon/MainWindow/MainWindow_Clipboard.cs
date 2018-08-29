using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Interface_Reception_Ribbon
{
    public partial class MainWindow
    {
        #region Definitions

        /// <summary> 剪贴板内容改变时API函数向windows发送的消息 </summary>
        const int WM_CLIPBOARDUPDATE = 0x031D;

        /// <summary> windows用于监视剪贴板的API函数 </summary>
        /// <param name="hwnd">要监视剪贴板的窗口的句柄</param>
        /// <returns>成功则返回true</returns>
        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary> 取消对剪贴板的监视 </summary>
        /// <param name="hwnd">监视剪贴板的窗口的句柄</param>
        /// <returns>成功则返回true</returns>
        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        #endregion

        #region CLIPBOARD

        /// <summary> WPF窗口重写 </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //添加监视消息事件
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            // HTodo  ：添加剪贴板监视 
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            AddClipboardFormatListener(handle);
        }

        /// <summary> 剪贴板监视事件 </summary>
        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CLIPBOARDUPDATE:
                    OnClipboardChanged();
                    break;
            }
            return IntPtr.Zero;
        }

        /// <summary> 剪贴板内容改变具体响应，根据需求，程序仅对文字内容进行监视 </summary>
        void OnClipboardChanged()
        {
            if (!Clipboard.ContainsText()) return;

            string text = string.Empty;
            try
            {
                text = Clipboard.GetText();
            }
            catch
            {
                return;
            }
            if (string.IsNullOrEmpty(text)) return;
            _pageRes.ReadResFromClipboard(text);
            return;
            /*ListBoxResItem res = _ResGetter.GetReservation(text);
            if (res != null) _pageRes.AddNewRes(res);
            Clipboard.SetText(string.Empty);*/
        }

        #endregion
    }
}
