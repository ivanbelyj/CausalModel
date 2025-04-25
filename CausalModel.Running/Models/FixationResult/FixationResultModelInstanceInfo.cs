using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.Models.FixationResult
{
    public class FixationResultModelInstanceInfo
    {
        public string ModelName { get; private set; }
        public string InstanceId { get; private set; }

        private FixationResultModelInstanceInfo(string modelName, string instanceId)
        {
            ModelName = modelName;
            InstanceId = instanceId;
        }

        public static FixationResultModelInstanceInfo Create<TFactValue>(ModelInstance<TFactValue> modelInstance)
            where TFactValue : class
        {
            return new FixationResultModelInstanceInfo(
                modelName: modelInstance.Model.Name,
                instanceId: modelInstance.InstanceId);
        }
    }
}
