using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{
    public class ModelProvider<TFactValue> : IModelProvider<TFactValue>
        where TFactValue : class
    {
        private readonly IResolvedModelProvider<TFactValue> resolvedModelProvider;
        private readonly string modelInstanceId;

        public ModelProvider(IResolvedModelProvider<TFactValue> modelProvider,
            string modelInstanceId)
        {
            resolvedModelProvider = modelProvider;
            this.modelInstanceId = modelInstanceId;
        }

        public IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts()
        {
            return resolvedModelProvider.GetInstanceFacts(modelInstanceId);
        }

        public InstanceFact<TFactValue> GetModelFact(string factId)
        {
            return resolvedModelProvider.GetFact(
                new InstanceFactAddress(factId, modelInstanceId));
        }
    }
}
