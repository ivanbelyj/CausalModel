using CausalModel.Facts;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation
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

        public void FixateFact(InstanceFactId instanceFactId, bool isOccurred)
        {
            if (resolvedModelProvider == null)
                throw new InvalidOperationException("Fixator is not initialized");

            var fact = resolvedModelProvider.GetFact(instanceFactId.ToAddress());
            FixateFact(fact, isOccurred);
        }

        public virtual void FixateFact(InstanceFact<TFactValue> fact, bool isOccurred)
        {
            factIdsFixated[fact.InstanceFactId] = isOccurred;
            FactFixated?.Invoke(this, fact, isOccurred);
        }
    }
}
