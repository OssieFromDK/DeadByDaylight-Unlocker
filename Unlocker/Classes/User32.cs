using Microsoft.VisualBasic.Logging;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;

namespace FortniteBurger.Classes
{
    public static partial class User32
    {
        [DllImport("user32.dll", EntryPoint = "FindWindowW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowSize(IntPtr hWnd, ref Rect lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPosition(IntPtr hWnd, IntPtr zOrder, int X, int Y, int CX, int CY, uint uFlags);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int WS_EX_TRANSPARENT = 32;
        public const int WS_EX_LAYERED = 524288;
        public const int GWL_EXSTYLE = -20;

        public static void MakeTransParent(IntPtr hWnd)
        {
            int extendedStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

            extendedStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED;

            SetWindowLong(hWnd, GWL_EXSTYLE, extendedStyle);
        }

    }
}
