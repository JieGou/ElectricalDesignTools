using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace WpfUI.Helpers;
public class WindowHelper
{
    [DllImport("user32.dll")]
    private static extern int ShowWindow(int hwnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    public static void SnapWindow(Window window, bool left)
    {
        int leftAdjust = 7;
        int rightAdjust = 15;
        int heightAdjust = 7;

        var win = new WindowInteropHelper(window);
        var hWnd = win.Handle;
        ShowWindow(1, 1);
        if (left) {
            MoveWindow(hWnd, 0 - leftAdjust, 0, Screen.PrimaryScreen.WorkingArea.Width / 2 + rightAdjust, Screen.PrimaryScreen.WorkingArea.Height + heightAdjust, true);
        }
        else {
            MoveWindow(hWnd, Screen.PrimaryScreen.WorkingArea.Width / 2 - leftAdjust, 0, Screen.PrimaryScreen.WorkingArea.Width / 2 + rightAdjust, Screen.PrimaryScreen.WorkingArea.Height + heightAdjust, true);
        }
    }
}
