using CausalModel.Blocks;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving.ResolvingNode
{
    /// <summary>
    /// When terms "local" and "mapped" ids are used, that's assumed relative 
    /// to the node root model by default.
    /// <para>
    /// Example:
    /// Root model represents a character, parent model - a story structure.
    /// The fact local id in our character root model
    /// is CharacterHasTrait and externally in the parent model it's addressed
    /// like EvaiHasTrait for a one of specific characters, used in the parent model.
    /// By default we choose a term for the fact type relative to the considered
    /// node root model. In the above described case the root model of the node
    /// is character model, so local fact id is CharacterHasTrait, mapped fact id - EvaiHasTrait.
    /// If the current node is the story model and character model is considered as block,
    /// then EvaiHasTrait should be called local external and CharacterHasTrait - external mapped
    /// </para>
    /// </summary>
    internal class ResolvedModelNode<TFactValue>
        where TFactValue : class
    {
        private readonly ModelInstance<TFactValue> modelInstance;
        private readonly BlockResolvingHandler<TFactValue> resolvingHandler;

        /// <summary>
        /// Indicates who am I in the external model. Null for the root model
        /// </summary>
        private readonly DeclaredBlock? declaredBlock;

        private readonly ResolvedModelNode<TFactValue>? parent;

        private readonly CachingDecorator<TFactValue> cachingDecorator;

        public ResolvedModelNode(
            ModelInstance<TFactValue> modelInstance,
            BlockResolvingHandler<TFactValue> resolvingHandler,
            DeclaredBlock? declaredBlock,
            ResolvedModelNode<TFactValue>? parent = null)
        {
            this.modelInstance = modelInstance;
            this.resolvingHandler = resolvingHandler;
            this.declaredBlock = declaredBlock;
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
            {
                throw new InvalidOperationException($"Fact (local id: {factLocalId}) "
                    + $"was not found in the model instance "
                    + $"(id: {modelInstance.InstanceId}).");
            }
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

        public InstanceFact<TFactValue>? TryGetCauseFromParent(InstanceFactAddress address)
        {
            if (parent == null)
            {
                return null;
                
            }

            if (declaredBlock == null)
            {
                throw new InvalidOperationException(
                    $"Resolved model has parent, but {nameof(declaredBlock)} is null. "
                    + "Block implementations should have this field set"
                );
            }

            return cachingDecorator
                .ExternalCauseIds
                .Where(localCauseId =>  localCauseId == address.FactId)
                .Select(declaredBlock.LocalCauseIdToMappedExternalFactId)
                .Where(mappedExternalFactId => mappedExternalFactId != null)
                .Select(parent.TryGetFact!)
                .Where(instanceFact => instanceFact != null)
                .FirstOrDefault();
        }

        /// <summary>
        /// Consequences of blocks that can be used by the node
        /// </summary>
        public InstanceFact<TFactValue>? TryGetConsequenceFromBlocks(InstanceFactAddress address)
        {
            var factsFromBlocks = new List<InstanceFact<TFactValue>>();

            var blocks = resolvingHandler.ResolvedBlockProviders;

            if (blocks == null)
            {
                return null;
            }

            // Fact ids from ExternalCauseIds:
            // 1. They were not found in the model => external for the node
            // (in terms of caching decorator)
            // 2. They are converted from local mapped to external local.
            // For example the fact id in the block
            // is CharacterHasTrait and externally in our root model it's addressed
            // like EvaiHasTrait for a one of specific characters, used in our model.
            // EvaiHasTrait is local mapped and should be converted to external local.

            // Facts cannot be obtained from the model by external ids directly,
            // so they must first be mapped to local
            return cachingDecorator
                .ExternalCauseIds
                .Where(externalCauseId => externalCauseId == address.FactId)
                .SelectMany(externalCauseId =>
                    blocks
                        .Select(
                            block =>
                                ExternalFactsResolvingUtils<TFactValue>
                                    .MappedLocalConsequenceIdToExternalLocalAndGetFactInRootInstance(block, externalCauseId)))
                .Where(fact => fact != null)
                .FirstOrDefault();
        }
    }
}
