﻿using PFTSUITemplate.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PFTSDesktop
{
    /// <summary>
    /// LoginDlg.xaml 的交互逻辑
    /// </summary>
    public partial class LoginDlg : WindowTemplet
    {
        public LoginDlg()
        {
            InitializeComponent();
        }

        private void main_Loaded(object sender, RoutedEventArgs e)
        {
            var debug = System.Configuration.ConfigurationManager.AppSettings["debug"];
            if (debug.ToLower() == "true")
            {
                PFTSTools.ConsoleManager.Toggle();
            }
        }
    }
}
