using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{
    internal class ResolvedModelNode<TFactValue>
        where TFactValue : class
    {
        private readonly ModelInstance<TFactValue> modelInstance;
        private readonly BlockResolvingHandler<TFactValue> resolvingHandler;
        private readonly ResolvedModelNode<TFactValue>? parent;

        private readonly CachingDecorator<TFactValue> cachingDecorator;

        public ResolvedModelNode(
            ModelInstance<TFactValue> modelInstance,
            BlockResolvingHandler<TFactValue> resolvingHandler,
            ResolvedModelNode<TFactValue>? parent = null)
        {
            this.modelInstance = modelInstance;
            this.resolvingHandler = resolvingHandler;
            this.parent = parent;

            cachingDecorator = new CachingDecorator<TFactValue>(modelInstance.Model);
        }

        public string ModelInstanceId => modelInstance.InstanceId;

        public InstanceFact<TFactValue>? TryGetFact(string factLocalId)
        {
            var fact = cachingDecorator.TryGetFact(factLocalId);
            return fact == null ? null : ToInstanceFact(fact);
        }

        public InstanceFact<TFactValue> GetFact(string factLocalId)
        {
            var res = TryGetFact(factLocalId);
            if (res == null)
                throw new InvalidOperationException($"Fact (local id: {factLocalId}) "
                    + $"was not found in the model instance "
                    + $"(id: {modelInstance.InstanceId}).");
            return res;
        }

        private InstanceFact<TFactValue> ToInstanceFact(Fact<TFactValue> fact)
        {
            return new InstanceFact<TFactValue>(fact, modelInstance.InstanceId);
        }
        private IEnumerable<InstanceFact<TFactValue>> ToInstanceFacts(
            IEnumerable<Fact<TFactValue>> facts)
        {
            return facts.Select(ToInstanceFact);
        }

        public IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts()
        {
            var modelFacts = ToInstanceFacts(modelInstance.Model.Facts);
            return modelFacts;
        }

        public IEnumerable<InstanceFact<TFactValue>>? TryGetExternalCauses()
        {
            IEnumerable<InstanceFact<TFactValue>> factsFromParent
                = new List<InstanceFact<TFactValue>>();

            if (parent != null)
                factsFromParent = cachingDecorator
                    .ExternalCauseIds
                    .Select(parent.TryGetFact)
                    .Where(parentFact => parentFact != null)!;

            var factsFromBlocks = new List<InstanceFact<TFactValue>>();

            var blocks = resolvingHandler.ResolvedBlockProviders;
            if (blocks != null)
            {
                foreach (var block in blocks)
                {
                    factsFromBlocks.AddRange(cachingDecorator
                        .ExternalCauseIds
                        .SelectMany(externalId => blocks
                            .Select(block => block.TryGetFactInRootInstance(externalId)))
                        .Where(fact => fact != null)!);
                }
            }

            return factsFromParent.Concat(factsFromBlocks);
        }
    }
}
