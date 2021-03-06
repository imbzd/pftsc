﻿using PFTSDesktop.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Data.Linq;

namespace PFTSDesktop.View.PoliceManager
{
    /// <summary>
    /// PoliceNewPage.xaml 的交互逻辑
    /// </summary>
    public partial class PoliceNewPage : Page
    {
        PFTSHwCtrl.PFTSZKFingerProxy m_fingerProxy;
        private bool m_bIsTimeToDie = false;
        private PoliceManagerViewModel m_model;

        public PoliceNewPage()
        {
            InitializeComponent();

            m_model = PoliceManagerViewModel.GetInstance();

            this.DataContext = m_model;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                m_fingerProxy = PFTSHwCtrl.PFTSZKFingerProxy.GetInstance();
                m_fingerProxy.FingerAcquire += M_fingerProxy_FingerAcquire;

            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void M_fingerProxy_FingerAcquire(Bitmap img, byte[] buffer, byte[] imgBuffer, int imageHeight, int imageWidth)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                imgFinger1.Source = PoliceNewPage.ChangeBitmapToImageSource(img);
                var officer = m_model.GetPoliceInfo;
                officer.fingerprint1 = new Binary(buffer);
                m_model.GetPoliceInfo = officer;
            });
        }

        private void DoCapture()
        {
            while (!m_bIsTimeToDie)
            {
                var b = m_fingerProxy.AcquireFingerprint();
                if (b)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        int size = 2048;
                        var img = m_fingerProxy.GetFingerImage();
                        var buffer = m_fingerProxy.GetRaw(ref size);
                        imgFinger1.Source = PoliceNewPage.ChangeBitmapToImageSource(img);

                        var officer = m_model.GetPoliceInfo;
                        officer.fingerprint1 = new Binary(buffer);
                        m_model.GetPoliceInfo = officer;
                    });
                }
                Thread.Sleep(200);
            }
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>  
        /// 从bitmap转换成ImageSource  
        /// </summary>  
        /// <param name="icon"></param>  
        /// <returns></returns>  
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            //Bitmap bitmap = icon.ToBitmap();  
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        /// <summary>
        /// 鼠标点击事件 - 左键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgFinger1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("1");
        }

        private void imgFinger1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("1");
        }
    }
}
