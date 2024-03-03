using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{
    public partial class ResolvedModelProvider<TFactValue> :
        IResolvedModelProvider<TFactValue>,
        IResolvedModelProviderFactory<TFactValue>
        where TFactValue : class
    {
        private readonly ResolvedModelNode<TFactValue> rootModel;

        private readonly InstanceFactAddressResolver addressResolver;
        private readonly BlockResolvingHandler<TFactValue> blockResolvingHandler;

        private readonly ResolvedModelProvider<TFactValue>? parent;

        protected ResolvedModelProvider(
            ModelInstance<TFactValue> modelInstance,
            IBlockResolver<TFactValue> blockResolver,
            ResolvedModelProvider<TFactValue>? parent)
        {
            this.parent = parent;

            blockResolvingHandler = new BlockResolvingHandler<TFactValue>(
                blockResolver,
                modelInstance,
                this);

            rootModel = new ResolvedModelNode<TFactValue>(
                modelInstance,
                blockResolvingHandler,
                parent?.rootModel);

            addressResolver = new InstanceFactAddressResolver(this);
        }

        public ResolvedModelProvider(ModelInstance<TFactValue> modelInstance,
            IBlockResolver<TFactValue> blockResolver)
            : this(modelInstance, blockResolver, null)
        {

        }

        private readonly Dictionary<string, ModelProvider<TFactValue>> 
            modelProvidersByInstanceId
            = new Dictionary<string, ModelProvider<TFactValue>>();

        public IModelProvider<TFactValue> GetModelProvider(string modelInstanceId)
        {
            if (!modelProvidersByInstanceId.ContainsKey(modelInstanceId))
            {
                var factProvider = new ModelProvider<TFactValue>(this,
                    modelInstanceId);
                modelProvidersByInstanceId.Add(modelInstanceId, factProvider);
            }

            return modelProvidersByInstanceId[modelInstanceId];
        }

        public string RootInstanceId => rootModel.ModelInstanceId;

        public virtual ResolvedModelProvider<TFactValue> CreateResolvedModel(
            ModelInstance<TFactValue> resolvedBlock,
            IBlockResolver<TFactValue> blockResolver)
        {
            return new ResolvedModelProvider<TFactValue>(resolvedBlock, blockResolver);
        }

        private InstanceFactId ResolveAddress(InstanceFactAddress address)
        {
            return addressResolver.Resolve(address);
        }

        public InstanceFact<TFactValue> GetFact(InstanceFactAddress address)
        {
            var res = TryGetFact(address);
            if (res == null)
                throw new InvalidOperationException($"Fact was not found " +
                    $"by address ({address}).");
            return res;
        }

        public IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts(
            string modelInstanceId)
        {
            var provider = GetInstanceProvider(modelInstanceId);
            return provider.rootModel.GetInstanceFacts();
        }

        public IEnumerable<InstanceFact<TFactValue>> GetResolvedFacts()
        {
            return blockResolvingHandler.ResolvedBlockProviders
                .SelectMany(provider =>
                    provider.rootModel.GetInstanceFacts())
                .Concat(rootModel.GetInstanceFacts());
        }

        public InstanceFact<TFactValue>? TryGetFact(InstanceFactAddress address)
        {
            var id = ResolveAddress(address);

            var provider = TryGetInstanceProvider(id.ModelInstanceId);
            var fact = provider?.rootModel.TryGetFact(id.FactId);

            //// Trying to get the fact from the parent
            //fact ??= this.parent?.rootModel.TryGetFact(id.FactId);
            return fact;
        }

        public InstanceFact<TFactValue>? TryGetFactInRootInstance(string factId)
        {
            return rootModel.TryGetFact(factId);
        }

        private ResolvedModelProvider<TFactValue>? TryGetInstanceProvider(
            string modelInstanceId)
        {
            if (rootModel.ModelInstanceId == modelInstanceId)
                return this;
            else if (parent != null &&
                parent.rootModel.ModelInstanceId == modelInstanceId)
            {
                return parent;
            }
            else
            {
                return blockResolvingHandler.TryGetResolvedBlock(modelInstanceId);
            }
        }

        private ResolvedModelProvider<TFactValue> GetInstanceProvider(
            string modelInstanceId)
        {
            var res = TryGetInstanceProvider(modelInstanceId);
            if (res == null)
                throw new InvalidOperationException($"Resolved model instance provider " +
                    $"(instance id: {modelInstanceId}) was not found.");
            return res;
        }

        //public IEnumerable<InstanceFact<TFactValue>>? TryGetExternalCauses(
        //    string modelInstanceId)
        //{
        //    var provider = TryGetInstanceProvider(modelInstanceId);
        //    return provider?.rootModel.TryGetExternalCauses();
        //}
    }
}
