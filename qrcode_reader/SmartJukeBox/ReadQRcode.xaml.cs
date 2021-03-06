﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using Microsoft.Phone.Tasks;

using ZXing;
using ZXing.Common;
using System.Threading;
using System.Threading.Tasks;
using SmartJukeBox.JsonTypes;

namespace SmartJukeBox
{
    public partial class ReadQRCode : PhoneApplicationPage
    {
        private readonly PhotoChooserTask photoChooserTask;
        private readonly BackgroundWorker scannerWorker;

        private BarcodeCaptureDevice _device;
        private DelayAction _resetTextAction;

        // Konstruktor
        public ReadQRCode()
        {
            InitializeComponent();

            _resetTextAction = new DelayAction(800, () => { DisplayResult(null); barCodeBorder.Child = null; });

            // prepare Photo Chooser Task for the open button
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += (s, e) => { if (e.TaskResult == TaskResult.OK) ProcessImage(e); };

            // prepare the backround worker thread for the image processing
            scannerWorker = new BackgroundWorker();
            scannerWorker.DoWork += scannerWorker_DoWork;
            scannerWorker.RunWorkerCompleted += scannerWorker_RunWorkerCompleted;

            // open the default barcode which should be displayed when the app starts
            var uri = new Uri("/images/35.png", UriKind.Relative);
            var imgSource = new BitmapImage(uri);
            BarcodeImage.Source = imgSource;
            imgSource.ImageOpened += (s, e) => scannerWorker.RunWorkerAsync(new WriteableBitmap((BitmapImage)s));
        }

        private void scannerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // processing the result of the background scanning
            //if (e.Cancelled)
            //{
            //    BarcodeContent.Text = "Cancelled.";
            //}
            //else if (e.Error != null)
            //{
            //    BarcodeContent.Text = e.Error.Message;
            //}
            //else
            //{
            //    var result = (Result)e.Result;
            //    DisplayResult(result);
            //}
        }

        private static void scannerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // scanning for a barcode
            e.Result = new BarcodeReader().Decode((WriteableBitmap)e.Argument);
        }

        private void ProcessImage(PhotoResult e)
        {
            // setting the image in the display and start scanning in the background
            var bmp = new BitmapImage();
            bmp.SetSource(e.ChosenPhoto);
            BarcodeImage.Source = bmp;
            scannerWorker.RunWorkerAsync(new WriteableBitmap(bmp));
        }

        private void OpenImageButton_Click(object sender, RoutedEventArgs e)
        {
            StopCamera();
            BarcodeImage.Visibility = System.Windows.Visibility.Visible;
            previewRect.Visibility = System.Windows.Visibility.Collapsed;

            photoChooserTask.Show();
        }

        private async void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            BarcodeImage.Visibility = System.Windows.Visibility.Collapsed;
            previewRect.Visibility = System.Windows.Visibility.Visible;
            await StartCamera();
        }

        private void BarcodeDetected(object sender, BarcodeDetectedEventArgs e)
        {
            barCodeBorder.Child = e.GetBarcodeBorderUIVideoUniformFill(barCodeBorder.ActualWidth, barCodeBorder.ActualHeight);
            DisplayResult(e.Result);
            if (_resetTextAction != null)
                _resetTextAction.Start();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            BarcodeImage.Visibility = System.Windows.Visibility.Collapsed;
            previewRect.Visibility = System.Windows.Visibility.Visible;
            await StartCamera();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            StopCamera();
            if (_resetTextAction != null)
            {
                _resetTextAction.Invoke();
            }
        }

        private async System.Threading.Tasks.Task StartCamera()
        {
            if (_device == null)
            {
                _device = new BarcodeCaptureDevice();
                _device.AutoFocus = true;
                _device.AutoDetectBarcode = true;
                await _device.InitAsync();
                previewTransform.Rotation = _device._device.SensorRotationInDegrees;
                barcodeUITransform.Rotation = _device._device.SensorRotationInDegrees;
                _device.BindVideoBrush(previewVideo);

                _device.BarcodeDetected += BarcodeDetected;
            }
        }

        private void StopCamera()
        {
            if (_device != null)
            {
                _device.Dispose();
                _device = null;
            }
        }

        private async void DisplayResult(Result result)
        {
            if (result == null)
            {
                return;
            }

            int spotIdIndex = result.Text.IndexOf("spotid=");
            string spotId = result.Text.Substring(spotIdIndex + 7, result.Text.Length - spotIdIndex - 7);

            var userSetSpot = await API.GetAsync<UserSetSpotResponse>(API.Actions.SetSpot, new string[] { Settings.UserID, spotId });

            if (userSetSpot.UserSetSpotResult)
            {
                App.ViewModel.SpotID = spotId;
                StopCamera();
                NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("It wasn't possible to check in.\nPlease make sure your wifi or mobile data connection is on.");
            }
        }

    //    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        if (BarcodeType.SelectedItem == null)
    //            return;

    //        IBarcodeWriter writer = new BarcodeWriter
    //        {
    //            Format = (BarcodeFormat)BarcodeType.SelectedItem,
    //            Options = new EncodingOptions
    //            {
    //                Height = 480,
    //                Width = 640
    //            }
    //        };
    //        var bmp = writer.Write(BarcodeContent.Text);
    //        BarcodeImage.Source = bmp;
    //    }
    }

    public class DelayAction : IDisposable
    {
        private static readonly int TickDuration = 100;
        private Action _action;
        private int _count;
        private bool _busy;
        private object _lock = new object();
        private bool _cancelFlag;
        private double _msDelay;

        public event EventHandler Completed;

        public DelayAction(double msDelay, Action action)
        {
            _action = action;
            _msDelay = msDelay;
        }

        public void Start()
        {
            _cancelFlag = false;
            lock (_lock)
            {
                _count = (int)(_msDelay / TickDuration);
            }
            if (_busy)
                return;
            else
                StartInternal();
        }

        public void Cancel()
        {
            if (!_busy)
                return;
            _cancelFlag = true;
        }

        public void Invoke()
        {
            Cancel();
            InvokeInternal();
        }

        private bool _isDisposed;
        public bool IsDisposed
        {
            get
            {
                lock (_lock)
                {
                    return _isDisposed;
                }
            }

            set
            {
                lock (_lock)
                {
                    _isDisposed = value;
                }
            }
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        private async void StartInternal()
        {
            _busy = true;
            SynchronizationContext sc = SynchronizationContext.Current;
            while (true)
            {
                await Task.Delay(TickDuration).ConfigureAwait(false);
                if (IsDisposed)
                    return;
                if (_cancelFlag)
                    return;
                lock (_lock)
                {
                    _count--;
                    if (_count <= 0)
                        break;
                }
            }
            sc.Post(new SendOrPostCallback(
                (obj) =>
                {
                    _busy = false;
                    if (_cancelFlag)
                        return;
                    InvokeInternal();
                }
                ), null
            );
        }

        private void InvokeInternal()
        {
            if (_action != null)
                _action();
            if (Completed != null)
                Completed(this, EventArgs.Empty);
        }

    }
}