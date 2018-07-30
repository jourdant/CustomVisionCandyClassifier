using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// 5ef01663-f198-4804-86e5-94f5c446f321_6e877eb0-f1e5-4062-9770-80a945bf5e82

namespace CustomVisionWinMLDemo
{
    public sealed class _x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82ModelInput
    {
        public VideoFrame data { get; set; }
    }

    public sealed class _x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82ModelOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> loss { get; set; }
        public _x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82ModelOutput()
        {
            this.classLabel = new List<string>();
            this.loss = new Dictionary<string, float>()
            {
                { "Mars", float.NaN },
                { "Snickers", float.NaN },
            };
        }
    }

    public sealed class _x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82Model
    {
        private LearningModelPreview learningModel;
        public static async Task<_x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82Model> Create_x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82Model(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            _x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82Model model = new _x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82Model();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<_x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82ModelOutput> EvaluateAsync(_x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82ModelInput input) {
            _x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82ModelOutput output = new _x0035_ef01663_x002D_f198_x002D_4804_x002D_86e5_x002D_94f5c446f321_6e877eb0_x002D_f1e5_x002D_4062_x002D_9770_x002D_80a945bf5e82ModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("data", input.data);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("loss", output.loss);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
