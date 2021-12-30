using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wallpaper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //var screenBounds = Screen.PrimaryScreen.Bounds;
            //this.Size = screenBounds.Size;
            //this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        public void SetImage(Bitmap bitmap)
        {
            this.pictureBox1.Image = bitmap;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.pictureBox1.Size = this.Size;
        }

        ///// <summary>
        ///// 设置程序开机启动
        ///// </summary>
        ///// <param name="strAppPath">应用程序exe所在文件夹</param>
        ///// <param name="strAppName">应用程序exe名称</param>
        ///// <param name="bIsAutoRun">自动运行状态</param>
        //private void SetAutoRun(string strAppPath, string strAppName, bool bIsAutoRun)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(strAppPath)
        //          || string.IsNullOrWhiteSpace(strAppName))
        //        {
        //            throw new Exception("应用程序路径或名称为空！");
        //        }
        //        RegistryKey reg = Registry.LocalMachine;
        //        RegistryKey run = reg.CreateSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\");
        //        if (bIsAutoRun)
        //        {
        //            run.SetValue(strAppName, strAppPath);
        //        }
        //        else
        //        {
        //            if (null != run.GetValue(strAppName))
        //            {
        //                run.DeleteValue(strAppName);
        //            }
        //        }
        //        run.Close();
        //        reg.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// 判断是否开机启动
        ///// </summary>
        ///// <param name="strAppPath">应用程序路径</param>
        ///// <param name="strAppName">应用程序名称</param>
        ///// <returns></returns>
        //private bool IsAutoRun(string strAppPath, string strAppName)
        //{
        //    try
        //    {
        //        RegistryKey reg = Registry.LocalMachine;
        //        RegistryKey software = reg.OpenSubKey(@"SOFTWARE");
        //        RegistryKey run = reg.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\");
        //        object key = run.GetValue(strAppName);
        //        software.Close();
        //        run.Close();
        //        if (null == key || !strAppPath.Equals(key.ToString()))
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex);
        //    }
        //}

    }
}
