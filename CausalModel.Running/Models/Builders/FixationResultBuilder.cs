using CausalModel.Blocks;
using CausalModel.Model.Instance;
using CausalModel.Running.Models.Common;
using CausalModel.Running.Models.FixationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.Models.Builders
{
    public class FixationResultBuilder<TFactValue>
        where TFactValue : class
    {
        private readonly Dictionary<string, FixationResultModelInstanceInfo<TFactValue>>
            modelInstanceInfoById = new Dictionary<string, FixationResultModelInstanceInfo<TFactValue>>();
        private readonly Dictionary<string, FixationResultModelInfo<FactInfo<TFactValue>, TFactValue>>
            modelInfoByName = new Dictionary<string, FixationResultModelInfo<FactInfo<TFactValue>, TFactValue>>();

        public void AddModelInstantiated(ModelInstance<TFactValue> modelInstance)
        {
            if (modelInstance == null)
            {
                throw new ArgumentNullException(nameof(modelInstance));
            }

            var modelName = modelInstance.Model.Name ?? throw new ArgumentException(
                nameof(modelInstance.Model),
                $"Causal model (id: {modelInstance.InstanceId}) must have its name set " +
                $"to be used in {nameof(FixationResultBuilder<TFactValue>)}");

            if (!modelInstanceInfoById.ContainsKey(modelInstance.InstanceId))
            {
                modelInstanceInfoById.Add(
                    modelInstance.InstanceId,
                    new FixationResultModelInstanceInfo<TFactValue>(modelInstance));
            }

            if (!modelInfoByName.TryGetValue(modelName, out var modelResult))
            {
                modelResult = new FixationResultModelInfo<FactInfo<TFactValue>, TFactValue>(modelName);
                modelInfoByName.Add(modelName, modelResult);
            }

            modelResult.RunCount++;
        }

        public void AddFact(InstanceFact<TFactValue> instanceFact, bool isHappened)
        {
            if (!modelInstanceInfoById.TryGetValue(
                instanceFact.InstanceFactId.ModelInstanceId,
                out var modelInstanceResult))
            {
                throw new ArgumentException(
                    nameof(instanceFact),
                    $"Invalid model of the instance fact (instance fact id: " +
                    $"{instanceFact.InstanceFactId}).");
            }

            string modelName = modelInstanceResult.ModelInstance.ModelName;

            string factLocalId = instanceFact.Fact.Id;

            var currentModelResult = modelInfoByName[modelName];

            if (!currentModelResult.FactsInfoByFactLocalId.ContainsKey(factLocalId))
            {
                currentModelResult.FactsInfoByFactLocalId.Add(factLocalId, new FactInfo<TFactValue>(factLocalId, instanceFact.Fact.FactValue));
            }

            if (isHappened)
            {
                currentModelResult.FactsInfoByFactLocalId[factLocalId].FactCount++;
            }

            (isHappened ? modelInstanceResult.OccurredFacts : modelInstanceResult.NotOccurredFacts)
                .Add(InstanceFactInfo<TFactValue>.Create(instanceFact));
        }

        public FixationResult<TFactValue> BuildResult(
            TimeSpan elapsed,
            int seed)
        {
            return new FixationResult<TFactValue>(elapsed)
            {
                Seed = seed,
                ModelInfoByName = modelInfoByName,
                ModelInstanceInfoById = modelInstanceInfoById,
            };
        }
    }
}
