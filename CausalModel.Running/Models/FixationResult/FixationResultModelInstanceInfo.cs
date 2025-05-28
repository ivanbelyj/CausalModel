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
    public class FixationResultModelInstanceInfo<TFactValue>
        where TFactValue : class
    {
        public ModelInstanceInfo ModelInstance { get; private set; }
        //public List<FixationResultImplementedBlockInfo> ImplementedBlocks { get; set; } = new List<FixationResultImplementedBlockInfo>();
        public List<InstanceFactInfo<TFactValue>> OccurredFacts { get; set; } = new List<InstanceFactInfo<TFactValue>>();
        public List<InstanceFactInfo<TFactValue>> NotOccurredFacts { get; set; } = new List<InstanceFactInfo<TFactValue>>();

        public FixationResultModelInstanceInfo(ModelInstance<TFactValue> modelInstance)
        {
            ModelInstance = ModelInstanceInfo.Create(modelInstance);
        }
    }
}
