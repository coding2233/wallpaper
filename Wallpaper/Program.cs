using FFmpeg.AutoGen;
using FFmpeg.AutoGen.Example;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Wallpaper
{
    class Program
    {
        private static IntPtr _nativeWallpapaerPtr;
        private static Process _videoProcess;
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var notifyIcon = new NotifyIcon();
            notifyIcon.Text = "Wallpaper";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("wallpaper.ico"))
            {
                notifyIcon.Icon = new Icon(stream);
            }
            notifyIcon.Visible = true;
            //菜单
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripItem item = contextMenuStrip.Items.Add("Exit");
            item.Click += (sender, e) =>
            {
                notifyIcon?.Dispose();
                notifyIcon = null;
                SetWallpaperBack();
                Application.Exit();
            };
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            Console.WriteLine("Hello World !");
            //ffmpeg
            ffmpeg.RootPath = "./";
            DecodeAllFramesToImages(AVHWDeviceType.AV_HWDEVICE_TYPE_NONE);
            //SetWallpaper();
            Application.Run();


        }

        private static void SetWallpaper()
        {
            string videoName = $"wallpaper_ffplay_video.mp4";
            try
            {
                _videoProcess = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "ffplay.exe";
                startInfo.Arguments = $"{videoName} -noborder -an -x 1920 -y 1080  -loop 0";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                _videoProcess.StartInfo = startInfo;
                _videoProcess.Start();
                Thread.Sleep(2000);
                ShowWindow(_videoProcess.MainWindowHandle, SW_SHOWMINIMIZED);
            }
            catch (Exception e)
            {
                SetWallpaperBack();
                Console.WriteLine(e);
            }
            _nativeWallpapaerPtr = IntPtr.Zero;
            IntPtr progmanPtr = FindWindow("Progman", "Program Manager");
            if (progmanPtr != IntPtr.Zero)
            {
                //win10
                SendMessage(progmanPtr, 0x52c, IntPtr.Zero, IntPtr.Zero);
                EnumWindows(EnumWindowProc, 0);
                if (_nativeWallpapaerPtr != IntPtr.Zero)
                {
                    SetParent(_videoProcess.MainWindowHandle, progmanPtr);
                    ShowWindow(_videoProcess.MainWindowHandle, SW_SHOWMAXIMIZED);
                    ShowWindow(_nativeWallpapaerPtr, SW_HIDE);
                    //ShowWindow(_videoProcess.MainWindowHandle, SW_SHOW);
                }
                else
                {
                    SetWallpaperBack();
                }
            }
          
        }

        private static void SetWallpaperBack()
        {
            if (_nativeWallpapaerPtr != IntPtr.Zero)
            {
                ShowWindow(_nativeWallpapaerPtr, SW_SHOW);
            }

            if (_videoProcess != null)
            {
                _videoProcess.Kill();
                _videoProcess?.Dispose();
                _videoProcess = null;
            }
        }

        private static bool EnumWindowProc(IntPtr hwnd, long lparam)
        {
            try
            {
                IntPtr sdPtr = FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", 0);
                if (sdPtr != IntPtr.Zero)
                {
                    StringBuilder classNameBuilder = new StringBuilder(1024);
                    int intClass = GetClassName(hwnd, classNameBuilder, 1024);
                    if ("WorkerW".Equals(classNameBuilder.ToString()))
                    {
                        IntPtr parentPtr = GetParent(hwnd);
                        // 找它的下一个窗口，类名为WorkerW，隐藏它
                        IntPtr hWorkerw = FindWindowEx(parentPtr, hwnd, "WorkerW", 0);
                        if (hWorkerw != IntPtr.Zero)
                        {
                            _nativeWallpapaerPtr = hWorkerw;
                            StringBuilder titleTextBuilder = new StringBuilder(1024);
                            int intText = GetWindowText(hwnd, titleTextBuilder, 1024);
                            //Console.WriteLine($"{hwnd} {intText} {titleTextBuilder.ToString()} {intClass} {classNameBuilder.ToString()}");
                            return false;
                        }
                        //ShowWindow(hWorkerw, SW_HIDE);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

        private static unsafe void DecodeAllFramesToImages(AVHWDeviceType HWDevice)
        {
            // decode all frames from url, please not it might local resorce, e.g. string url = "../../sample_mpeg4.mp4";
            var url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"; // be advised this file holds 1440 frames
            using (var vsd = new VideoStreamDecoder(url, HWDevice))
            {
                Console.WriteLine($"codec name: {vsd.CodecName}");

                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = vsd.PixelFormat;
                var destinationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                using (var vfc =
                    new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                {
                    var frameNumber = 0;

                    while (vsd.TryDecodeNextFrame(out var frame))
                    {
                        var convertedFrame = vfc.Convert(frame);

                        using (var bitmap = new Bitmap(convertedFrame.width,
                            convertedFrame.height,
                            convertedFrame.linesize[0],
                            PixelFormat.Format24bppRgb,
                            (IntPtr)convertedFrame.data[0]))
                            bitmap.Save($"frame.{frameNumber:D8}.jpg", ImageFormat.Jpeg);

                        Console.WriteLine($"frame: {frameNumber}");
                        frameNumber++;
                    }
                }
            }
        }

  

        #region user32.dll
        delegate bool WindowEnumProc(IntPtr hwnd, long lparam);
        [DllImport("user32.dll")]
        static extern bool EnumWindows(WindowEnumProc lpEnumFunc,long lparam);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hwnd, IntPtr childAfter,string lpszClassName,int lpszWindow);
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString,int maxCount);
        [DllImport("user32.dll")]
        static extern int GetClassName(IntPtr hwnd, StringBuilder lpString, int maxCount);
        [DllImport("user32.dll")]
        static extern IntPtr GetParent(IntPtr hwnd);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr child, IntPtr parent);
        [DllImport("user32.dll")]
        static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
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

        #region SetWindowLong 参数
        const int GWL_STYLE = -16;  //边框用的
        const int WS_BORDER = 1;
        const int WS_POPUP = 0x800000;
        #endregion

        #endregion
    }
}
