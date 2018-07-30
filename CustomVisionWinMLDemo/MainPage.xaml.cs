using CustomVisionWinMLDemo.ML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CustomVisionWinMLDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaCapture _webcam;
        MediaFrameReader _frameReader;
        CustomVisionImageClassifier _model;

        SoftwareBitmap backBuffer;
        bool _taskRunning = false;

        public MainPage()
        {
            this.InitializeComponent();

            //initialise model
            _model = new CustomVisionImageClassifier("ms-appx:///Assets/6e877eb0f1e54062977080a945bf5e82.onnx");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //load model
            await _model.LoadModelAsync();


            //using Windows.Media.Capture.MediaCapture APIs to stream from webcam
            _webcam = new MediaCapture();
            await _webcam.InitializeAsync();

            //frame reader
            _frameReader = await _webcam.CreateFrameReaderAsync(_webcam.FrameSources.FirstOrDefault().Value, MediaEncodingSubtypes.Argb32);
            _frameReader.FrameArrived += _frameReader_FrameArrived; ;
            await _frameReader.StartAsync();

            //start capture preview
            outputImageCaptureElement.Source = _webcam;
            outputImageCaptureElement.FlowDirection = FlowDirection.LeftToRight;
            await _webcam.StartPreviewAsync();
        }

        private async void _frameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            if (_taskRunning == true) return;

            using (InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream())
            {
                _taskRunning = true;
                var imgFormat = ImageEncodingProperties.CreateBmp();
                var previewProperties = _webcam.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
                await _webcam.CapturePhotoToStreamAsync(imgFormat, randomAccessStream);
                randomAccessStream.Seek(0L);

                await EvaluateFromImageAsync(randomAccessStream);
                _taskRunning = false;
            }
        }

        private async Task EvaluateFromImageAsync(IRandomAccessStream stream)
        {
            var result = await _model.EvaluateModelAsync(stream.CloneStream());

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    //handle object detection on UI thread
                    var highestProbability = result.Loss.Where(l => l.Value > 0.65f).ToList();
                    outputLabel.Text = highestProbability.Count > 0 ? highestProbability.FirstOrDefault().Key : "No result";

                    var label = $"Evaluation Time: {result.EvaluationTimeMilliseconds}ms\r\n";
                    label += string.Join("\r\n", result.Loss.Select(l => l.Key + ": " + l.Value));
                    outputDebugLabel.Text = label;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });
        }
    }
}
