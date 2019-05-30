//此类仅仅封装MS示例代码

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using StringDict = System.Collections.Generic.Dictionary<string, string>;

namespace Ledros
{
    public class WPFClipboard
    {
        #region Private fields

        // 下一个监视者
        static IntPtr _hwndNextWatcher;
        static HwndSource _hwndSource;
        static bool _isWatching;

        public class ClipboardChangedEventArgs : EventArgs
        {
            public string ResNumber { get; internal set; }

            public ClipboardChangedEventArgs(string resNumber)
            {
                this.ResNumber = resNumber;
            }
        }

        public delegate void ClipboardChangedHandle(object sender, ClipboardChangedEventArgs e);

        public static event ClipboardChangedHandle ClipboardChanged;

        #endregion

        #region Clipboard watcher related methods

        public static void InitClipboardWatcher(Window window)
        {
            if (_isWatching) return;
            WindowInteropHelper wih = new WindowInteropHelper(Window.GetWindow(window));
            _hwndSource = HwndSource.FromHwnd(wih.Handle);
            _hwndSource.AddHook(PageProc);   // 添加钩子获取系统消息
            // 将传入的窗口设置为监视者
            _hwndNextWatcher = Win32.SetClipboardViewer(_hwndSource.Handle);
            _isWatching = true;
        }

        static void CloseClipboardWatcher()
        {
            // 将当前监视者从队列中移除
            Win32.ChangeClipboardChain(_hwndSource.Handle, _hwndNextWatcher);
            _hwndNextWatcher = IntPtr.Zero;
            _hwndSource.RemoveHook(PageProc);
            _isWatching = false;
        }

        static IntPtr PageProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_CHANGECBCHAIN:
                    if (wParam == _hwndNextWatcher)
                    {
                        // 监视者队列改变
                        _hwndNextWatcher = lParam;
                    }
                    else if (_hwndNextWatcher != IntPtr.Zero)
                    {
                        // 将消息分发至下一个监视者
                        Win32.SendMessage(_hwndNextWatcher, msg, wParam, lParam);
                    }
                    break;

                case Win32.WM_DRAWCLIPBOARD:
                    // 剪贴板内容改变
                    string data = null;
                    try
                    {
                        data = Clipboard.GetText();
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(data))
                        {
                            string gains = Booking.Sow(data);
                            if (gains != null && gains != "DataError")
                                Clipboard.SetText("");
                            else

                            ClipboardChanged?.Invoke(null,
                                new ClipboardChangedEventArgs(gains));
                        }
                        
                    }
                    // 将消息分发至下一个监视者
                    Win32.SendMessage(_hwndNextWatcher, msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        
        #endregion

    }
}
