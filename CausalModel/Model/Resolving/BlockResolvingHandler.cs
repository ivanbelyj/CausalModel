using CausalModel.Blocks.Resolving;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving;
internal class BlockResolvingHandler<TFactValue>
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
        IResolvedModelProviderFactory<TFactValue> resolvedModelProviderFactory
        )
	{
        this.blockResolver = blockResolver;
        this.modelInstance = modelInstance;
        this.resolvedModelProviderFactory = resolvedModelProviderFactory;
        resolvedBlocksByInstanceId = new(GetResolvedBlocksByInstanceIds);
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
        foreach (var resolvedBlock in resolvedBlocksList)
        {
            var resolvedModelProvider = resolvedModelProviderFactory
                .CreateResolvedBlock(resolvedBlock, blockResolver);
            resolvedBlocks.Add(resolvedBlock.InstanceId, resolvedModelProvider);
        }

        return resolvedBlocks;
    }

    public ResolvedModelProvider<TFactValue>? TryGetResolvedBlock(
        string modelInstanceId)
    {
        resolvedBlocksByInstanceId.Value.TryGetValue(modelInstanceId, out var res);
        return res;
    }

    /// <summary>
    /// Returns resolved blocks of the root causal model instance
    /// (not recursive resolving)
    /// </summary>
    private IEnumerable<ModelInstance<TFactValue>> GetResolvedBlocks()
    {
        var res = new List<ModelInstance<TFactValue>>();
        foreach (BlockFact block in modelInstance.Model.BlockFacts)
        {
            ModelInstance<TFactValue> resolvedBlock =
                blockResolver.Resolve(block.Block, modelInstance);

            res.Add(resolvedBlock);
        }
        return res;
    }
}

