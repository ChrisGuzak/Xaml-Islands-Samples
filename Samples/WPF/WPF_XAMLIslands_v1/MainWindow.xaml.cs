﻿using Helpers;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.ApplicationModel.DataTransfer;

namespace WPF_XAMLIslands_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Using WinRT DataTransferManager in Win32
            IntPtr hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;

            var dtm = DataTransferManagerHelper.GetForWindow(hwnd);
            dtm.DataRequested += OnDataRequested;
            UWPApplication.App.ShowShareUIForWindow += ShowShareUI;

            //Detect Orientation
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            //Pass the WinForm's Hwmd to the UWP Application
            (UWPApplication.App.Current as UWPApplication.App).WindowHandle = hwnd;

        }

        #region Using WinRT DataTransferManager in Win32
        string _text;
        private void ShowShareUI(string text)
        {
            IntPtr hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            DataTransferManagerHelper.ShowShareUIForWindow(hwnd);
            _text = text;
        }
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();

            try
            {
                DataPackage dp = args.Request.Data;
                dp.Properties.Title = _text;
                dp.SetText(_text);
            }
            finally
            {
                deferral.Complete();
            }
        }
        #endregion

        #region Detect Orientation

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (SystemParameters.PrimaryScreenWidth > SystemParameters.PrimaryScreenHeight)
            {
                UWPApplication.App.OrientationChanged(new UWPApplication.OrientationChangedEventArgs() { IsLandscape = true });
            }
            else
            {
                UWPApplication.App.OrientationChanged(new UWPApplication.OrientationChangedEventArgs() { IsLandscape = false });

            }
        }
        #endregion
    }
}
