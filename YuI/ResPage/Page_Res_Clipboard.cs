/****************************** Module Header ******************************\ 
* Module Name:  MainWindow.xaml.cs
* Project:      CSWPFClipboardViewer
* Copyright (c) Microsoft Corporation.
* 
* The CSWPFClipboardViewer project provides the sample on how to monitor
* Windows clipboard changes in a WPF application.

 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
 All other rights reserved.

 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Windows;
using System.Windows.Interop;

namespace YuI
{
    /// <summary>
    /// Main window of the application, also will be used to get clipboard messages.
    /// </summary>
    public partial class Page_Reservation
    {
        #region Private fields

        /// <summary>
        /// Next clipboard viewer window 
        /// </summary>
        private IntPtr _hwndNextViewer;

        /// <summary>
        /// The <see cref="HwndSource"/> for this window.
        /// </summary>
        private HwndSource _hwndSource;

        private bool _isViewing;

        #endregion

        #region Clipboard viewer related methods

        private void InitializeClipboardViewer()
        {
            if (_isViewing) return;
            WindowInteropHelper wih = new WindowInteropHelper(Window.GetWindow(this));
            _hwndSource = HwndSource.FromHwnd(wih.Handle);

            _hwndSource.AddHook(this.PageProc);   // start processing window messages
            _hwndNextViewer = Win32.SetClipboardViewer(_hwndSource.Handle);   // set this window as a viewer
            _isViewing = true;
        }

        private void CloseClipboardViewer()
        {
            // remove this window from the clipboard viewer chain
            Win32.ChangeClipboardChain(_hwndSource.Handle, _hwndNextViewer);

            _hwndNextViewer = IntPtr.Zero;
            _hwndSource.RemoveHook(this.PageProc);
            _isViewing = false;
        }
        
        private IntPtr PageProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_CHANGECBCHAIN:
                    if (wParam == _hwndNextViewer)
                    {
                        // clipboard viewer chain changed, need to fix it.
                        _hwndNextViewer = lParam;
                    }
                    else if (_hwndNextViewer != IntPtr.Zero)
                    {
                        // pass the message to the next viewer.
                        Win32.SendMessage(_hwndNextViewer, msg, wParam, lParam);
                    }
                    break;

                case Win32.WM_DRAWCLIPBOARD:
                    // clipboard content changed
                    this.SniffRes();
                    // pass the message to the next viewer.
                    Win32.SendMessage(_hwndNextViewer, msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        #endregion

    }
}
