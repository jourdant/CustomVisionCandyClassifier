using System.Collections.Generic;

namespace CustomVisionWinMLDemo.ML
{
    public class CustomVisionImageClassification
    {
        public int EvaluationTimeMilliseconds { get; set; }

        public IList<string> ClassLabel { get; set; }
        public IDictionary<string, float> Loss { get; set; }
        public CustomVisionImageClassification()
        {
            ClassLabel = new List<string>();
            Loss = new Dictionary<string, float>()
            {
                { "Mars" ,float.NaN },
                {"Snickers", float.NaN }
            };
        }
    }
}
