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

namespace CausalModel.Model.Resolving;
public partial class ResolvedModelProvider<TFactValue> : IResolvedModelProvider<TFactValue>
{
    private readonly ResolvedModelNode<TFactValue> rootModel;
    private readonly IBlockResolver<TFactValue> blockResolver;

    // Todo: remove Lazy ?
    private readonly Lazy<Dictionary<string, ResolvedModelProvider<TFactValue>>>
        resolvedBlocksByInstanceId;

    private readonly InstanceFactAddressResolver addressResolver;

    protected ResolvedModelProvider(ModelInstance<TFactValue> modelInstance,
        IBlockResolver<TFactValue> blockResolver,
        ResolvedModelProvider<TFactValue>? parent)
    {
        this.blockResolver = blockResolver;

        rootModel = new ResolvedModelNode<TFactValue>(
            modelInstance,
            blockResolver,
            resolvedBlocksByInstanceId
                ?.Value
                .Values
                .Select(x => x.rootModel),
            parent?.rootModel);
        resolvedBlocksByInstanceId = new(GetResolvedBlocksByInstanceIds);
        addressResolver = new InstanceFactAddressResolver(this);
    }

    public ResolvedModelProvider(ModelInstance<TFactValue> modelInstance,
        IBlockResolver<TFactValue> blockResolver)
        : this(modelInstance, blockResolver, null)
    {

    }

    private readonly Dictionary<string, ModelProvider<TFactValue>>
        modelProviderByInstanceId = new();

    public IModelProvider<TFactValue> GetModelProvider(string modelInstanceId)
    {
        if (!modelProviderByInstanceId.ContainsKey(modelInstanceId))
        {
            var factProvider = new ModelProvider<TFactValue>(this,
                modelInstanceId);
            modelProviderByInstanceId.Add(modelInstanceId, factProvider);
        }

        return modelProviderByInstanceId[modelInstanceId];
    }

    public IModelProvider<TFactValue> GetRootModelProvider()
    {
        return GetModelProvider(rootModel.ModelInstanceId);
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

    private InstanceFactId ResolveAddress(InstanceFactAddress address)
    {
        return addressResolver.Resolve(address);
    }

    public InstanceFact<TFactValue> GetFact(InstanceFactAddress address)
    {
        var res = TryGetFact(address);
        if (res == null)
            throw new InvalidOperationException($"Fact  was not found " +
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
        return resolvedBlocksByInstanceId
            .Value
            .Values
            .SelectMany(provider =>
                provider.rootModel.GetInstanceFacts())
            .Concat(rootModel.GetInstanceFacts());
    }

    public InstanceFact<TFactValue>? TryGetFact(InstanceFactAddress address)
    {
        var id = ResolveAddress(address);

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

    public IEnumerable<InstanceFact<TFactValue>>? TryGetExternalCauses(
        string modelInstanceId)
    {
        var provider = GetInstanceProvider(modelInstanceId);
        return provider.rootModel.TryGetExternalCauses();
    }
}
