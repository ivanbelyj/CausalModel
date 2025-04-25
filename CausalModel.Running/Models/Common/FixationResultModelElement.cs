using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.Models.Common
{
    /// <summary>
    /// Part of the fixation result, separated by model
    /// </summary>
    public class FixationResultModelElement<TFactInfo, TFactValue>
        where TFactInfo : FactInfo<TFactValue>
        where TFactValue : class
    {
        public string ModelName { get; private set; }
        public int RunCount { get; set; }

        //public Dictionary<string, int> FactCountsByFactLocalId { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, TFactInfo> FactsInfoByFactLocalId { get; set; } = new Dictionary<string, TFactInfo>();

        public FixationResultModelElement(string modelName)
        {
            ModelName = modelName;
        }
    }
}
