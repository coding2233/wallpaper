using FFmpeg.AutoGen;
using FFmpeg.AutoGen.Example;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wallpaper
{
    class Program
    {
        private static IntPtr _nativeWallpapaerPtr;
        private static Process _exeProcess;
        private static List<Form1> _form1s;
        private static HashSet<string> _videoTask = new HashSet<string>();
        #region path

        private static string UserPath
        {
            get
            {
                string userPath = Environment.GetEnvironmentVariable("USERPROFILE");
                if (string.IsNullOrEmpty(userPath))
                {
                    userPath = "./";
                }
                userPath = Path.Combine(userPath, ".wallpaper");
                if (!Directory.Exists(userPath))
                {
                    Directory.CreateDirectory(userPath);
                }
                return userPath;
            }
        }

        private static string VideoAddressConfig
        {
            get
            {
                string userPath = Environment.GetEnvironmentVariable("USERPROFILE");
                if (string.IsNullOrEmpty(userPath))
                {
                    userPath = "./";
                }
                userPath = Path.Combine(userPath, ".wallpaper");
                if (!Directory.Exists(userPath))
                {
                    Directory.CreateDirectory(userPath);
                }
                userPath = Path.Combine(userPath, ".wallpaper.address");
                return userPath;
            }
        }

        private static string WallpaperAddress
        {
            get
            {
                if (File.Exists(VideoAddressConfig))
                {
                    string address = File.ReadAllText(VideoAddressConfig);
                    if (!string.IsNullOrEmpty(address))
                    {
                        if (File.Exists(address))
                        {
                            return address;
                        }
                    }
                }
                return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (File.Exists(VideoAddressConfig))
                    {
                        File.Delete(VideoAddressConfig);
                    }
                }
                else
                {
                    if (File.Exists(VideoAddressConfig))
                    {
                        File.Delete(VideoAddressConfig);
                    }
                    if (File.Exists(value))
                    {
                        File.WriteAllText(VideoAddressConfig, value);
                    }
                }
            }
        }


        private static bool AutoStartup
        {
            get
            {
                Microsoft.Win32.RegistryKey rk2 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                return rk2.GetValue("Wallpaper") != null;
            }
            set
            {
                string path = Application.ExecutablePath;
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;
                string softWareKey = "Wallpaper";
                Microsoft.Win32.RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                //var rk2Object = rk2.GetValue(softWareKey);
                //bool autoStart = (rk2Object == null || !rk2Object.ToString().Equals(path));
                if (value)
                {
                    rk2.SetValue(softWareKey, path);
                }
                else
                {
                    if(rk2.GetValue(softWareKey)!=null)
                        rk2.DeleteValue(softWareKey);
                }
                //rk2.DeleteValue("Wallpaper", false);
                rk2.Close();
                rk.Close();
            }
        }

        #endregion

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
            //??????
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            var asItem = AddNotifyIconItem(contextMenuStrip, "Auto startup", (sender, e) => {
                string path = Application.ExecutablePath;
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;
                try
                {
                    var tsmi = sender as ToolStripMenuItem;
                    bool oldAutoStartup = tsmi.Checked;
                    AutoStartup = !oldAutoStartup;
                    if (AutoStartup != oldAutoStartup)
                    {
                        notifyIcon.ShowBalloonTip(500, "Auto startup", oldAutoStartup? "Turn off autostart" : "Startup setup succeeded.", ToolTipIcon.Info);
                        tsmi.Checked = !oldAutoStartup;
                    }
                    else
                    {
                        notifyIcon.ShowBalloonTip(500, "Auto startup", $"Startup Settings failed. Procedure.", ToolTipIcon.Error);
                    }
                }
                catch (Exception ee)
                {
                    notifyIcon.ShowBalloonTip(500, "Auto startup", $"Startup Settings failed. Procedure. {ee}", ToolTipIcon.Error);
                }
            });
            (asItem as ToolStripMenuItem).Checked = AutoStartup;
            AddNotifyIconItem(contextMenuStrip, "Select wallpaper", (sender, e) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Video files|*.*|Exe files|*.exe";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string oldWallpaperAddress = WallpaperAddress;
                    WallpaperAddress = openFileDialog.FileName;
                    if (WallpaperAddress.Equals(oldWallpaperAddress))
                    {
                        return;
                    }
                    if (!string.IsNullOrEmpty(oldWallpaperAddress))
                    {
                        if (_videoTask.Contains(oldWallpaperAddress))
                            _videoTask.Remove(oldWallpaperAddress);
                        SetWallpaperBack();
                    }
                    SetWallpaper(WallpaperAddress);
                    notifyIcon.ShowBalloonTip(500, "Update wallpap", "Wallpaper setup successfully.", ToolTipIcon.Info);
                }
            });
            AddNotifyIconItem(contextMenuStrip, "Exit", (sender, e) =>
            {
                notifyIcon?.Dispose();
                notifyIcon = null;
                SetWallpaperBack();
                Application.Exit();
            });
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            Console.WriteLine("Hello World !");
            //SetWallpaper();
            if (!string.IsNullOrEmpty(WallpaperAddress))
            {
                notifyIcon.Text += $"\n{Path.GetFileName(WallpaperAddress)}";
                SetWallpaper(WallpaperAddress);
            }
            else
            {
                notifyIcon.ShowBalloonTip(2000, "Select wallpaper", "Please select a video file or exe program as wallpaper.", ToolTipIcon.Info);
            }

            Application.Run();
        }
        private static void SetWallpaper(string wallpaperAddress)
        {
            _nativeWallpapaerPtr = IntPtr.Zero;
            IntPtr progmanPtr = FindWindow("Progman", "Program Manager");
            if (progmanPtr != IntPtr.Zero)
            {
                //win10
                SendMessage(progmanPtr, 0x52c, IntPtr.Zero, IntPtr.Zero);
                EnumWindows(EnumWindowProc, 0);
                if (_nativeWallpapaerPtr != IntPtr.Zero)
                {
                    //var child = FindWindowEx(progmanPtr,IntPtr.Zero, "UnityWndClass", 0);
                    //SetParent(child, IntPtr.Zero);
                    try
                    {
                        if (wallpaperAddress.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                        {
                            RunExe(wallpaperAddress, progmanPtr);
                        }
                        else
                        {
                            PlayVideo(wallpaperAddress, progmanPtr);
                        }
                        ShowWindow(_nativeWallpapaerPtr, SW_HIDE);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        SetWallpaperBack();
                    }
                }
                else
                {
                    SetWallpaperBack();
                }
            }
          
        }

        private static void SetWallpaperBack()
        {
            if (_exeProcess != null)
            {
                _exeProcess.Kill();
                _exeProcess = null;
            }

            if (_form1s != null)
            {
                while (_form1s.Count > 0)
                {
                    var form = _form1s[0];
                    _form1s.RemoveAt(0);
                    form.Close();
                }
            }

            if (_nativeWallpapaerPtr != IntPtr.Zero)
            {
                ShowWindow(_nativeWallpapaerPtr, SW_SHOW);
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
                        // ????????????????????????????????????WorkerW????????????
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

        private static unsafe Task DecodeAllFramesToImages(string videoUrl,AVHWDeviceType HWDevice,Action<Bitmap> onBitmapCallback,bool loop=false)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                Bitmap cacheBitmap = null;
                string videoAddress = videoUrl;
                videoUrl = Path.GetFullPath(videoUrl);
                while (_videoTask.Contains(videoAddress))
                {
                    try
                    {
                        // decode all frames from url, please not it might local resorce, e.g. string url = "../../sample_mpeg4.mp4";
                        //var url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"; // be advised this file holds 1440 frames
                        using (var vsd = new VideoStreamDecoder(videoUrl, HWDevice))//""
                        {
                            Console.WriteLine($"codec name: {vsd.CodecName}");

                            var info = vsd.GetContextInfo();
                            info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                            var sourceSize = vsd.FrameSize;
                            var sourcePixelFormat = vsd.PixelFormat;
                            var screenBounds = Screen.PrimaryScreen.Bounds;
                            var destinationSize = screenBounds.Size; // sourceSize;
                            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;

                            using (var vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                            {
                                var frameNumber = 0;
                                int waitTime = 1000 / 60;
                                while (_videoTask.Contains(videoAddress)&&vsd.TryDecodeNextFrame(out var frame))
                                {
                                    var convertedFrame = vfc.Convert(frame);
                                    var bitmap = new Bitmap(convertedFrame.width,
                                        convertedFrame.height,
                                        convertedFrame.linesize[0],
                                        PixelFormat.Format24bppRgb,
                                        (IntPtr)convertedFrame.data[0]);

                                    if (cacheBitmap == null)
                                    {
                                        string cachePath =Path.Combine(UserPath, $"{Path.GetFileName(videoUrl)}.cacheBitmap");
                                        if (File.Exists(cachePath))
                                        {
                                            File.Delete(cachePath);
                                        }
                                        bitmap.Save(cachePath, ImageFormat.Jpeg);
                                        cacheBitmap = new Bitmap(cachePath);
                                    }

                                    onBitmapCallback(bitmap);

                                    Thread.Sleep(waitTime);
                                    Console.WriteLine($"frame: {frameNumber}");
                                    frameNumber++;
                                }

                                onBitmapCallback(cacheBitmap);
                            }
                        }
                        if (!loop)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            });
        }

        private static ToolStripItem AddNotifyIconItem(ContextMenuStrip contextMenuStrip, string text, EventHandler eventHandler)
        {
            var item = contextMenuStrip.Items.Add(text);
            item.Click += eventHandler;
            //(item as ToolStripMenuItem).Checked = true;
            return item;
        }

        private static void PlayVideo(string wallpaperAddress,IntPtr progmanPtr)
        {
            bool isShowForms = false;
            List<Form1> forms = new List<Form1>();
            foreach (var item in Screen.AllScreens)
            {
                Form1 form1 = new Form1();

                form1.Bounds = item.Bounds;
                //form1.Location = item.Bounds.Location;
                //form1.Size = item.Bounds.Size;
                forms.Add(form1);
            }
            _form1s = forms;
            //ffmpeg
            ffmpeg.RootPath = Path.GetDirectoryName(Application.ExecutablePath);
            //http://ivi.bupt.edu.cn/hls/cctv6hd.m3u8 
            //./wallpaper_ffplay_video.mp4
            _videoTask.Add(wallpaperAddress);
            DecodeAllFramesToImages(wallpaperAddress, AVHWDeviceType.AV_HWDEVICE_TYPE_NONE, (bitmap) => {
                foreach (var item in forms)
                {
                    item.SetImage(bitmap);
                    isShowForms = true;
                }
            }, true);

            while (!isShowForms)
            {
                Thread.Sleep(100);
            }
            foreach (var form1 in forms)
            {
                SetParent(form1.Handle, progmanPtr);
                form1.Show();
            }
        }

        private static void RunExe(string wallpaperAddress, IntPtr progmanPtr)
        {
            _exeProcess = new Process();
            _exeProcess.StartInfo.FileName = wallpaperAddress;
            _exeProcess.StartInfo.UseShellExecute = true;
            _exeProcess.StartInfo.CreateNoWindow = true;
            _exeProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            _exeProcess.Start();
            Thread.Sleep(2000);
            SetParent(_exeProcess.MainWindowHandle, progmanPtr);
            SetWindowLong(_exeProcess.MainWindowHandle, GWL_STYLE,WS_BORDER);
            ShowWindow(_exeProcess.MainWindowHandle, SW_SHOWMAXIMIZED);
            var screenBounds = Screen.PrimaryScreen.Bounds;
            SetWindowPos(_exeProcess.MainWindowHandle, IntPtr.Zero, screenBounds.X-10, screenBounds.Y-10, screenBounds.Width, screenBounds.Height, 0x0040);
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
        static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd,IntPtr hWndInsertAfter,int x,int y,int width,int height,uint flag);

        #region ShowWindow ?????????????????????????????????
        /// <summary>
        /// ?????????????????????????????????
        /// </summary>
        internal const int SW_HIDE = 0;

        /// <summary>
        /// ???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
        /// </summary>
        internal const int SW_SHOWNORMAL = 1;

        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        internal const int SW_SHOWMINIMIZED = 2;
        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        internal const int SW_SHOWMAXIMIZED = 3;

        /// <summary>
        /// ???????????????????????????????????????????????????????????????SW_SHOWNORMAL????????????????????????????????????
        /// </summary>
        internal const int SW_SHOWNOACTIVATE = 4;

        /// <summary>
        /// ???????????????????????????????????????????????????????????????
        /// </summary>
        internal const int SW_SHOW = 5;

        /// <summary>
        /// ???????????????????????????????????????Z??????????????????????????????
        /// </summary>
        internal const int SW_MINIMIZE = 6;

        /// <summary>
        /// ??????????????????????????????????????????SW_SHOWMINIMIZED????????????????????????????????????
        /// </summary>
        internal const int SW_SHOWMINNOACTIVE = 7;

        /// <summary>
        /// ????????????????????????????????????????????????SW_SHOW????????????????????????????????????
        /// </summary>
        internal const int SW_SHOWNA = 8;

        /// <summary>
        /// ????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
        /// </summary>
        internal const int SW_RESTORE = 9;

        /// <summary>
        /// ?????????STARTUPINFO??????????????????SW_FLAG???????????????????????????STARTUPINFO ????????????????????????????????????????????????CreateProcess?????????
        /// </summary>
        internal const int SW_SHOWDEFAULT = 10;

        /// <summary>
        /// ?????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
        /// </summary>
        internal const int SW_FORCEMINIMIZE = 11;
        #endregion

        #region SetWindowLong ??????
        const int GWL_STYLE = -16;  //????????????
        const int WS_BORDER = 1;
        const int WS_POPUP = 0x800000;
        #endregion

        #endregion
    }
}
