using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{
    internal class BlockResolvingHandler<TFactValue>
        where TFactValue : class
    {
        private readonly IBlockResolver<TFactValue> blockResolver;
        private readonly ModelInstance<TFactValue> modelInstance;
        private readonly IResolvedModelProviderFactory<TFactValue>
            resolvedModelProviderFactory;
        private readonly Lazy<Dictionary<string, ResolvedModelProvider<TFactValue>>>
            resolvedBlocksByInstanceId;

        public IEnumerable<ResolvedModelProvider<TFactValue>> ResolvedBlockProviders
            => resolvedBlocksByInstanceId.Value.Values;

        public BlockResolvingHandler(
            IBlockResolver<TFactValue> blockResolver,
            ModelInstance<TFactValue> modelInstance,
            IResolvedModelProviderFactory<TFactValue> resolvedModelProviderFactory)
        {
            this.blockResolver = blockResolver;
            this.modelInstance = modelInstance;
            this.resolvedModelProviderFactory = resolvedModelProviderFactory;
            resolvedBlocksByInstanceId
                = new Lazy<Dictionary<string, ResolvedModelProvider<TFactValue>>>(
                    GetResolvedBlocksByInstanceIds);
        }

        public ResolvedModelProvider<TFactValue>? TryGetResolvedBlock(
            string modelInstanceId)
        {
            resolvedBlocksByInstanceId.Value.TryGetValue(modelInstanceId, out var res);
            return res;
        }

        /// <summary>
        /// Returns a dictionary of resolved blocks of the causal model
        /// (not recursive resolving)
        /// </summary>
        private Dictionary<string, ResolvedModelProvider<TFactValue>>
            GetResolvedBlocksByInstanceIds()
        {
            var resolvedBlocks
                = new Dictionary<string, ResolvedModelProvider<TFactValue>>();

            var resolvedBlocksList = GetResolvedBlocks();
            foreach (var (declaredBlock, resolvedBlock) in resolvedBlocksList)
            {
                var resolvedModelProvider = resolvedModelProviderFactory.CreateResolvedModel(
                    resolvedBlock,
                    declaredBlock,
                    blockResolver);
                resolvedBlocks.Add(resolvedBlock.InstanceId, resolvedModelProvider);
            }

            return resolvedBlocks;
        }

        /// <summary>
        /// Returns resolved blocks of the root causal model instance
        /// (not recursive resolving)
        /// </summary>
        private IEnumerable<(DeclaredBlock, ModelInstance<TFactValue>)> GetResolvedBlocks()
        {
            var res = new List<(DeclaredBlock, ModelInstance<TFactValue>)>();
            foreach (DeclaredBlock declaredBlock in modelInstance.Model.DeclaredBlocks)
            {
                ModelInstance<TFactValue> resolvedBlock =
                    blockResolver.Resolve(declaredBlock, modelInstance);

                res.Add((declaredBlock, resolvedBlock));
            }
            return res;
        }
    }
}
