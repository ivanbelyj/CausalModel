using CausalModel.Facts;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation.Fixators
{
    public class Fixator<TFactValue> : IFixator<TFactValue>
        where TFactValue : class
    {
        private Dictionary<InstanceFactId, bool> factIdsFixated =
            new Dictionary<InstanceFactId, bool>();
        private IResolvedModelProvider<TFactValue>? resolvedModelProvider;

        public event FactFixatedEventHandler<TFactValue>? FactFixated;

        public void Initialize(IResolvedModelProvider<TFactValue> resolvedModelProvider)
        {
            this.resolvedModelProvider = resolvedModelProvider;
        }

        public bool? IsFixated(InstanceFactId id)
        {
            bool isInDict = factIdsFixated.TryGetValue(id, out var isOccurred);
            if (isInDict)
                return isOccurred;
            else
                return null;
        }

        public void HandleFixation(InstanceFactId instanceFactId, bool isOccurred)
        {
            var fact = GetFactById(instanceFactId);
            HandleFixation(fact, isOccurred);
        }

        public virtual void HandleFixation(
            InstanceFact<TFactValue> fact,
            bool isOccurred)
        {
            Fixate(fact, isOccurred);
        }

        protected void Fixate(InstanceFactId instanceFactId, bool isOccurred)
        {
            var fact = GetFactById(instanceFactId);
            Fixate(fact, isOccurred);
        }

        protected void Fixate(
            InstanceFact<TFactValue> fact,
            bool isOccurred)
        {
            factIdsFixated[fact.InstanceFactId] = isOccurred;
            FactFixated?.Invoke(this, fact, isOccurred);
        }

        private InstanceFact<TFactValue> GetFactById(InstanceFactId instanceFactId)
        {
            if (resolvedModelProvider == null)
                throw new InvalidOperationException("Fixator is not initialized");

            return resolvedModelProvider.GetFact(instanceFactId.ToAddress());
        }
    }
}
