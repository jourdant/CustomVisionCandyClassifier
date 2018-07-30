using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CustomVisionWinMLDemo.ML
{
    public class CustomVisionImageClassifier
    {
        string _modelUri;
        bool _isLoaded = false;

        LearningModelPreview _model;
        ImageVariableDescriptorPreview _inputImageDescription;
        string _outputTensorDescription;


        public CustomVisionImageClassifier(string modelUri)
        {
            _modelUri = modelUri;
        }

        public async Task LoadModelAsync()
        {
            if (_isLoaded == true) return;

            //load model from assets
            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(_modelUri));
            _model = await LearningModelPreview.LoadModelFromStorageFileAsync(modelFile);

            var inputFeatures = _model.Description.InputFeatures.ToList();
            var outputFeatures = _model.Description.OutputFeatures.ToList();

            //read properties out of the model for binding
            _inputImageDescription = (ImageVariableDescriptorPreview)inputFeatures.FirstOrDefault();
            _isLoaded = true;
        }

        public async Task<CustomVisionImageClassification> EvaluateModelAsync(IRandomAccessStream randomAccessStream)
        {
            if (_model != null)
            {
                try
                {
                    //prepare input image
                    var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
                    var transform = new BitmapTransform() { ScaledHeight = _inputImageDescription.Height, ScaledWidth = _inputImageDescription.Width, InterpolationMode = BitmapInterpolationMode.Linear };
                    var inputImage = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);

                    //convert to image format required by the model
                    if (inputImage.BitmapPixelFormat != _inputImageDescription.BitmapPixelFormat)
                        inputImage = SoftwareBitmap.Convert(inputImage, _inputImageDescription.BitmapPixelFormat);

                    //bind inputs
                    var binding = new LearningModelBindingPreview(_model as LearningModelPreview);
                    if (_inputImageDescription.ModelFeatureKind == LearningModelFeatureKindPreview.Image)
                        binding.Bind(_inputImageDescription.Name, VideoFrame.CreateWithSoftwareBitmap(inputImage));

                    //bind outputs
                    CustomVisionImageClassification output = new CustomVisionImageClassification();
                    binding.Bind("classLabel", output.ClassLabel);
                    binding.Bind("loss", output.Loss);

                    //run model
                    var startTime = DateTime.Now;
                    var results = await _model.EvaluateAsync(binding, string.Empty);
                    var diff = DateTime.Now.Subtract(startTime).TotalMilliseconds;

                    //return result
                    output.EvaluationTimeMilliseconds = (int)diff;
                    return output;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            throw new Exception("Underlying model is not initialized. ");
        }
    }
}
