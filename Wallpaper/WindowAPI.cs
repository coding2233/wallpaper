using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Wallpaper
{
    internal class WindowsAPI
    {

        const int WM_MOUSEWHEEL = 0x020A; //鼠标滚轮
        const int WM_LBUTTONDOWN = 0x0201;//鼠标左键
        const int WM_LBUTTONUP = 0x0202;
        const int WM_KEYDOWN = 0x0100;//模拟按键
        const int WM_KEYUP = 0x0101;
        const int MOUSEEVENTF_MOVE = 0x0001;//用于琴台鼠标移动
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;//前台鼠标单击
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int WM_SETTEXT = 0x000C;//设置文字
        const int WM_GETTEXT = 0x000D;//读取文字

        #region ShowWindow 方法窗体状态的参数枚举
        /// <summary>
        /// 隐藏窗口并激活其他窗口
        /// </summary>
        internal const int SW_HIDE = 0;

        /// <summary>
        /// 激活并显示一个窗口。如果窗口被最小化或最大化，系统将其恢复到原来的尺寸和大小。应用程序在第一次显示窗口的时候应该指定此标志
        /// </summary>
        internal const int SW_SHOWNORMAL = 1;

        /// <summary>
        /// 激活窗口并将其最小化
        /// </summary>
        internal const int SW_SHOWMINIMIZED = 2;
        /// <summary>
        /// 激活窗口并将其最大化
        /// </summary>
        internal const int SW_SHOWMAXIMIZED = 3;

        /// <summary>
        /// 以窗口最近一次的大小和状态显示窗口。此值与SW_SHOWNORMAL相似，只是窗口没有被激活
        /// </summary>
        internal const int SW_SHOWNOACTIVATE = 4;

        /// <summary>
        /// 在窗口原来的位置以原来的尺寸激活和显示窗口
        /// </summary>
        internal const int SW_SHOW = 5;

        /// <summary>
        /// 最小化指定的窗口并且激活在Z序中的下一个顶层窗口
        /// </summary>
        internal const int SW_MINIMIZE = 6;

        /// <summary>
        /// 最小化的方式显示窗口，此值与SW_SHOWMINIMIZED相似，只是窗口没有被激活
        /// </summary>
        internal const int SW_SHOWMINNOACTIVE = 7;

        /// <summary>
        /// 以窗口原来的状态显示窗口。此值与SW_SHOW相似，只是窗口没有被激活
        /// </summary>
        internal const int SW_SHOWNA = 8;

        /// <summary>
        /// 激活并显示窗口。如果窗口最小化或最大化，则系统将窗口恢复到原来的尺寸和位置。在恢复最小化窗口时，应用程序应该指定这个标志
        /// </summary>
        internal const int SW_RESTORE = 9;

        /// <summary>
        /// 依据在STARTUPINFO结构中指定的SW_FLAG标志设定显示状态，STARTUPINFO 结构是由启动应用程序的程序传递给CreateProcess函数的
        /// </summary>
        internal const int SW_SHOWDEFAULT = 10;

        /// <summary>
        /// 最小化窗口，即使拥有窗口的线程被挂起也会最小化。在从其他线程最小化窗口时才使用这个参数
        /// </summary>
        internal const int SW_FORCEMINIMIZE = 11;
        #endregion

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        internal static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(WindowEnumProc func, IntPtr lParam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
        [DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        internal static extern IntPtr FindWindow(string className, string windowName);
        //[DllImport("User32.dll", EntryPoint = "SendMessage")]
        //internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        //[DllImport("User32.dll", EntryPoint = "SendMessage")]
        //internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, string lParam);
        [DllImport("user32.dll")]//在窗口列表中寻找与指定条件相符的第一个子窗口
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, // handle to parent window
                                                int hwndChildAfter, // handle to child window
                                                string className, //窗口类名            
                                                string windowName);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, string lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetClassName(IntPtr hWnd, string lpString, int nMaxCount);

        [DllImport("gdi32.dll")]
        internal static extern uint GetPixel(int hdc, int nXPos, int nYPos);

        [System.Runtime.InteropServices.DllImport("user32")]
        internal static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateDC(
         string lpszDriver,         // driver name驱动名
         string lpszDevice,         // device name设备名
         string lpszOutput,         // not used; should be NULL
         IntPtr lpInitData   // optional printer data
         );
        [DllImport("gdi32.dll")]
        internal static extern int BitBlt(
         IntPtr hdcDest, // handle to destination DC目标设备的句柄
         int nXDest,   // x-coord of destination upper-left corner目标对象的左上角的X坐标
         int nYDest,   // y-coord of destination upper-left corner目标对象的左上角的Y坐标
         int nWidth,   // width of destination rectangle目标对象的矩形宽度
         int nHeight, // height of destination rectangle目标对象的矩形长度
         IntPtr hdcSrc,   // handle to source DC源设备的句柄
         int nXSrc,    // x-coordinate of source upper-left corner源对象的左上角的X坐标
         int nYSrc,    // y-coordinate of source upper-left corner源对象的左上角的Y坐标
         UInt32 dwRop   // raster operation code光栅的操作值
         );
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleDC(
         IntPtr hdc // handle to DC
         );
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleBitmap(
         IntPtr hdc,         // handle to DC
         int nWidth,      // width of bitmap, in pixels
         int nHeight      // height of bitmap, in pixels
         );
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SelectObject(
         IntPtr hdc,           // handle to DC
         IntPtr hgdiobj    // handle to object
         );
        [DllImport("gdi32.dll")]
        internal static extern int DeleteDC(
         IntPtr hdc           // handle to DC
         );
        [DllImport("user32.dll")]
        internal static extern bool PrintWindow(
         IntPtr hwnd,                // Window to copy,Handle to the window that will be copied.
         IntPtr hdcBlt,              // HDC to print into,Handle to the device context.
         UInt32 nFlags               // Optional flags,Specifies the drawing options. It can be one of the following values.
         );
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowDC(
         IntPtr hwnd
         );
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(int hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int Left; //最左坐标
            internal int Top; //最上坐标
            internal int Right; //最右坐标
            internal int Bottom; //最下坐标
        }
        //internal static Bitmap GetWindowCapture(IntPtr hWnd)
        //{
        //    IntPtr hscrdc = GetWindowDC(hWnd);
        //    RECT windowRect = new RECT();
        //    GetWindowRect(hWnd, ref windowRect);
        //    int width = windowRect.Right - windowRect.Left;
        //    int height = windowRect.Bottom - windowRect.Top;
        //    IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, width, height);
        //    IntPtr hmemdc = CreateCompatibleDC(hscrdc);
        //    SelectObject(hmemdc, hbitmap);
        //    PrintWindow(hWnd, hmemdc, 0);
        //    Bitmap bmp = Bitmap.FromHbitmap(hbitmap);
        //    DeleteDC(hscrdc);//删除用过的对象
        //    DeleteDC(hmemdc);//删除用过的对象
        //    return bmp;
        //}
    }
}
