using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.ResolvingModelProvider;
public class ResolvedModelProvider<TFactValue> : IResolvedModelProvider<TFactValue>
{
    private readonly RootModelDecorator<TFactValue> rootModel;
    private readonly IBlockResolver<TFactValue> blockResolver;

    private readonly Lazy<Dictionary<string, ResolvedModelProvider<TFactValue>>>
        resolvedBlocksByInstanceId;

    protected ResolvedModelProvider(ModelInstance<TFactValue> modelInstance,
        IBlockResolver<TFactValue> blockResolver,
        ResolvedModelProvider<TFactValue>? parent)
    {
        rootModel = new RootModelDecorator<TFactValue>(modelInstance,
            blockResolver, parent?.rootModel);

        this.blockResolver = blockResolver;

        resolvedBlocksByInstanceId = new(GetResolvedBlocksByInstanceIds);
    }

    public ResolvedModelProvider(ModelInstance<TFactValue> modelInstance,
        IBlockResolver<TFactValue> blockResolver)
        : this(modelInstance, blockResolver, null)
    {
        
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

        var resolvedBlocksList = rootModel.GetResolvedBlocks();
        foreach (var resolvedBlock in resolvedBlocksList)
        {
            var resolvedModelProvider = CreateResolvedBlock(resolvedBlock,
                blockResolver);
            resolvedBlocks.Add(resolvedBlock.InstanceId, resolvedModelProvider);
        }

        return resolvedBlocks;
    }

    protected virtual ResolvedModelProvider<TFactValue> CreateResolvedBlock(
        ModelInstance<TFactValue> resolvedBlock,
        IBlockResolver<TFactValue> blockResolver)
    {
        return new ResolvedModelProvider<TFactValue>(resolvedBlock, blockResolver);
    }

    public InstanceFact<TFactValue> GetFact(InstanceFactId id)
    {
        var res = TryGetFact(id);
        if (res == null)
            throw new InvalidOperationException($"Fact (id: {id}) was not found.");
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
        return resolvedBlocksByInstanceId
            .Value
            .Values
            .SelectMany(provider =>
                provider.rootModel.GetInstanceFacts())
            .Concat(rootModel.GetInstanceFacts());
    }

    public InstanceFact<TFactValue>? TryGetFact(InstanceFactId id)
    {
        var provider = GetInstanceProvider(id.ModelInstanceId);
        return provider.rootModel.TryGetFact(id.FactId);
    }

    private ResolvedModelProvider<TFactValue> GetInstanceProvider(
        string modelInstanceId)
    {
        if (rootModel.IsOwn(modelInstanceId))
            return this;
        else
        {
            return resolvedBlocksByInstanceId.Value[modelInstanceId];
        }
    }

    public IEnumerable<InstanceFact<TFactValue>> GetExternalCauses(
        string modelInstanceId)
    {
        var provider = GetInstanceProvider(modelInstanceId);
        return provider.rootModel.GetExternalCauses();
    }
}
