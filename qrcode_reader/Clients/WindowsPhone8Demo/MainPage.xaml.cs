/*
 * Copyright 2012 ZXing.Net authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
using WindowsPhone8Demo.Resources;
using Microsoft.Devices;
using Microsoft.Phone.Tasks;

using ZXing;
using ZXing.Common;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsPhone8Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly PhotoChooserTask photoChooserTask;
        private readonly BackgroundWorker scannerWorker;

        private BarcodeCaptureDevice _device;
        private DelayAction _resetTextAction;

        private bool _cameraFlag;

        // Konstruktor
        public MainPage()
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

            foreach (var x in typeof(BarcodeFormat).GetFields())
            {
                if (x.IsLiteral)
                {
                    BarcodeType.Items.Add(x.GetValue(null));
                }
            }

            // open the default barcode which should be displayed when the app starts
            var uri = new Uri("/images/35.png", UriKind.Relative);
            var imgSource = new BitmapImage(uri);
            BarcodeImage.Source = imgSource;
            imgSource.ImageOpened += (s, e) => scannerWorker.RunWorkerAsync(new WriteableBitmap((BitmapImage)s));
        }

        private void scannerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // processing the result of the background scanning
            if (e.Cancelled)
            {
                BarcodeContent.Text = "Cancelled.";
            }
            else if (e.Error != null)
            {
                BarcodeContent.Text = e.Error.Message;
            }
            else
            {
                var result = (Result)e.Result;
                DisplayResult(result);
            }
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
            _cameraFlag = false;
            BarcodeImage.Visibility = System.Windows.Visibility.Visible;
            previewRect.Visibility = System.Windows.Visibility.Collapsed;

            photoChooserTask.Show();
        }

        private async void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            BarcodeImage.Visibility = System.Windows.Visibility.Collapsed;
            previewRect.Visibility = System.Windows.Visibility.Visible;
            _cameraFlag = true;
            await StartCamera();
        }

        private void BarcodeDetected(object sender, BarcodeDetectedEventArgs e)
        {
            barCodeBorder.Child = e.GetBarcodeBorderUIVideoUniformFill(barCodeBorder.ActualWidth, barCodeBorder.ActualHeight);
            DisplayResult(e.Result);
            if (_resetTextAction != null)
                _resetTextAction.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_cameraFlag)
                StartCamera();
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

        private void DisplayResult(Result result)
        {
            if (result != null)
            {
                BarcodeType.SelectedItem = result.BarcodeFormat;
                BarcodeContent.Text = result.Text;
            }
            else
            {
                BarcodeType.SelectedItem = null;
                BarcodeContent.Text = "No barcode found.";
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (BarcodeType.SelectedItem == null)
                return;

            IBarcodeWriter writer = new BarcodeWriter
            {
                Format = (BarcodeFormat)BarcodeType.SelectedItem,
                Options = new EncodingOptions
                {
                    Height = 480,
                    Width = 640
                }
            };
            var bmp = writer.Write(BarcodeContent.Text);
            BarcodeImage.Source = bmp;
        }
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