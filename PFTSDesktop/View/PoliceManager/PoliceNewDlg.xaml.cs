﻿using PFTSDesktop.ViewModel;
using PFTSUITemplate.Controls;
using System;
using System.Collections.Generic;
using System.Data.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PFTSDesktop.View.PoliceManager
{
    /// <summary>
    /// PoliceNewDlg.xaml 的交互逻辑
    /// </summary>
    public partial class PoliceNewDlg : WindowTemplet
    {
        PFTSHwCtrl.PFTSZKFingerProxy m_fingerProxy;
        private bool m_bIsTimeToDie = false;
        private PoliceManagerViewModel m_model;
        private int m_idx = -1;
        public PoliceNewDlg()
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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void M_fingerProxy_FingerAcquire(Bitmap img, byte[] buffer, byte[] imgBuffer, int imageHeight, int imageWidth)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                if (m_idx == 1)
                {
                    imgFinger1.Source = ChangeBitmapToImageSource(img);
                    var officer = m_model.GetPoliceInfo;
                    officer.fingerprint1 = new Binary(buffer);
                    
                    m_model.GetPoliceInfo = officer;
                    bool bExist = false;
                    for (int i = 0; i < officer.officer_fingerprint.Count; i++)
                    {
                        if (officer.officer_fingerprint[i].finger_id == 1)
                        {
                            officer.officer_fingerprint[i].img = new System.Data.Linq.Binary(imgBuffer);
                            bExist = true;
                            break;
                        }
                    }
                    if (!bExist)
                    {
                        PFTSModel.officer_fingerprint fp = new PFTSModel.officer_fingerprint();
                        fp.finger_id = 1;
                        fp.officer_id = officer.id;
                        fp.img_height = imageHeight;
                        fp.img_width = imageWidth;
                        fp.img = new System.Data.Linq.Binary(imgBuffer);
                        officer.officer_fingerprint.Add(fp);
                    }
                }
                else if (m_idx == 2)
                {
                    imgFinger2.Source = ChangeBitmapToImageSource(img);
                    var officer = m_model.GetPoliceInfo;
                    officer.fingerprint2 = new Binary(buffer);
                    m_model.GetPoliceInfo = officer;
                    bool bExist = false;
                    for (int i = 0; i < officer.officer_fingerprint.Count; i++)
                    {
                        if (officer.officer_fingerprint[i].finger_id == 2)
                        {
                            officer.officer_fingerprint[i].img = new System.Data.Linq.Binary(imgBuffer);
                            bExist = true;
                            break;
                        }
                    }
                    if (!bExist)
                    {
                        PFTSModel.officer_fingerprint fp = new PFTSModel.officer_fingerprint();
                        fp.finger_id = 2;
                        fp.officer_id = officer.id;
                        fp.img = new System.Data.Linq.Binary(imgBuffer);
                        fp.img_height = imageHeight;
                        fp.img_width = imageWidth;
                        officer.officer_fingerprint.Add(fp);
                    }
                }
            });
        }

        //private void DoCapture()
        //{
        //    while (!m_bIsTimeToDie)
        //    {
        //        var b = m_fingerProxy.AcquireFingerprint();
        //        if (b)
        //        {
        //            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
        //            {
        //                int size = 2048;
        //                if (m_idx == 1)
        //                {
        //                    var img = m_fingerProxy.GetFingerImage();
        //                    var buffer = m_fingerProxy.GetRaw(ref size);
        //                    imgFinger1.Source = PoliceNewPage.ChangeBitmapToImageSource(img);

        //                    var officer = m_model.GetPoliceInfo;
        //                    officer.fingerprint1 = new Binary(buffer);
        //                    m_model.GetPoliceInfo = officer;
        //                }
        //                else if (m_idx == 2)
        //                {
        //                    var img = m_fingerProxy.GetFingerImage();
        //                    var buffer = m_fingerProxy.GetRaw(ref size);
        //                    imgFinger2.Source = PoliceNewPage.ChangeBitmapToImageSource(img);

        //                    var officer = m_model.GetPoliceInfo;
        //                    officer.fingerprint2 = new Binary(buffer);
        //                    m_model.GetPoliceInfo = officer;
        //                }
        //            });
        //        }
        //        Thread.Sleep(200);
        //    }
        //}

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
            imgFinger1_Border.BorderBrush = System.Windows.Media.Brushes.Blue;
            imgFinger2_Border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0xab, 0xad, 0xb3));
            m_idx = 1;
        }

        private void imgFinger2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgFinger1_Border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0xab, 0xad, 0xb3));
            imgFinger2_Border.BorderBrush = System.Windows.Media.Brushes.Blue;
            m_idx = 2;
        }
    }
}
