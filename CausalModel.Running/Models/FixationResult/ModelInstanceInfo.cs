using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.Models.FixationResult
{
    public class ModelInstanceInfo
    {
        public string ModelName { get; private set; }
        public string InstanceId { get; private set; }

        private ModelInstanceInfo(string modelName, string instanceId)
        {
            ModelName = modelName;
            InstanceId = instanceId;
        }

        public static ModelInstanceInfo Create<TFactValue>(ModelInstance<TFactValue> modelInstance)
            where TFactValue : class
        {
            return new ModelInstanceInfo(
                modelName: modelInstance.Model.Name,
                instanceId: modelInstance.InstanceId);
        }
    }
}
