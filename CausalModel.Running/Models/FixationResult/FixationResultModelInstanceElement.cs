using CausalModel.Blocks;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.Models.FixationResult
{
    /// <summary>
    /// Part of the fixation result, separated by model instance
    /// </summary>
    public class FixationResultModelInstanceElement<TFactValue>
        where TFactValue : class
    {
        public FixationResultModelInstanceInfo ModelInstance { get; private set; }
        //public List<FixationResultImplementedBlockInfo> ImplementedBlocks { get; set; } = new List<FixationResultImplementedBlockInfo>();
        public List<FixationResultInstanceFactInfo<TFactValue>> OccurredFacts { get; set; } = new List<FixationResultInstanceFactInfo<TFactValue>>();
        public List<FixationResultInstanceFactInfo<TFactValue>> NotOccurredFacts { get; set; } = new List<FixationResultInstanceFactInfo<TFactValue>>();

        public FixationResultModelInstanceElement(ModelInstance<TFactValue> modelInstance)
        {
            ModelInstance = FixationResultModelInstanceInfo.Create(modelInstance);
        }
    }
}
