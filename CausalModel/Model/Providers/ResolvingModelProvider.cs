using CausalModel.Blocks.Resolving;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using System.Linq;

namespace CausalModel.Model.Providers;

/// <summary>
/// Resolving blocks and provides causal model data
/// </summary>
public class ResolvingModelProvider<TFactValue> : IResolvedModelProvider<TFactValue>
{
    private readonly IBlockResolver<TFactValue> resolver;
    private readonly Lazy<Dictionary<string, ResolvingModelProvider<TFactValue>>>
        resolvedBlocksByInstanceId;

    private readonly ModelInstance<TFactValue> rootInstance;

    // For more optimized accessing to the root model instance
    private readonly CausalModelWrapper<TFactValue> modelWrapper;

    public ResolvingModelProvider(ModelInstance<TFactValue> causalInstance,
        IBlockResolver<TFactValue> blockResolver)
    {
        resolver = blockResolver;
        resolvedBlocksByInstanceId = new(GetResolvedBlocks);

        rootInstance = causalInstance;
        modelWrapper = new CausalModelWrapper<TFactValue>(causalInstance.Model);
    }

    public string RootModelInstanceId => rootInstance.InstanceId;

    /// <summary>
    /// Returns a dictionary of resolved blocks of the causal model
    /// (not recursive resolving)
    /// </summary>
    /// <returns>Resolving model providers by ids</returns>
    private Dictionary<string, ResolvingModelProvider<TFactValue>> GetResolvedBlocks()
    {
        var resolvedBlocks
            = new Dictionary<string, ResolvingModelProvider<TFactValue>>();

        foreach (BlockFact block in modelWrapper.Blocks)
        {
            ModelInstance<TFactValue> resolvedBlock =
                resolver.Resolve(block.Block, rootInstance);

            resolvedBlocks.Add(resolvedBlock.InstanceId,
                new ResolvingModelProvider<TFactValue>(resolvedBlock, resolver));
        }
        return resolvedBlocks;
    }

    ///// <summary>
    ///// Finds something via function in the provider and its blocks (not recursively)
    ///// </summary>
    //private T? Find<T>(Func<ResolvingModelProvider<TFactValue>, T?> func)
    //    where T : class
    //{
    //    T? res = func(this);

    //    if (res != null)
    //        return res;
    //    else
    //    {
    //        // Todo: should resolve here?
    //        if (resolvedBlocksByInstanceId == null)
    //            resolvedBlocksByInstanceId = GetResolvedBlocks();

    //        foreach (var resolvedBlock in resolvedBlocksByInstanceId.Values)
    //        {
    //            var resInBlock = func(resolvedBlock);
    //            if (resInBlock != null)
    //                return resInBlock;
    //        }
    //    }

    //    return null;
    //}

    public InstanceFact<TFactValue> GetFact(InstanceFactId id)
    {
        var res = TryGetFact(id);
        if (res == null)
            throw new InvalidOperationException($"Fact (id: {id}) was not found.");
        return res;
    }

    public InstanceFact<TFactValue>? TryGetFactInRootModel(string factLocalId)
    {
        var fact = modelWrapper.TryGetFact(factLocalId);
        return fact == null ? null : ToInstanceFact(fact);
    }

    private T? TryGetInRootAndChildren<T>(
        Func<ResolvingModelProvider<TFactValue>, T?> func,
        string causalInstanceId)
    {
        if (causalInstanceId == rootInstance.InstanceId)
            return func(this);

        return func(resolvedBlocksByInstanceId
            .Value[causalInstanceId]);
    }

    private T GetInRootAndChildren<T>(
        Func<ResolvingModelProvider<TFactValue>, T?> func,
        string causalInstanceId)
    {
        var res = TryGetInRootAndChildren(func, causalInstanceId);
        if (res == null)
            throw new InvalidOperationException($"Required item was not found in "
                + $"causal model instance (id: {causalInstanceId}) "
                + $"or this instance does not exist.");
        return res;
    }

    public InstanceFact<TFactValue>? TryGetFact(InstanceFactId id)
    {
        return TryGetInRootAndChildren((provider) =>
        {
            return provider.TryGetFactInRootModel(id.FactId);
        }, id.CausalInstanceId);
    }

    private InstanceFact<TFactValue> ToInstanceFact(Fact<TFactValue> fact)
    {
        return new InstanceFact<TFactValue>(fact, rootInstance.InstanceId);
    }

    private IEnumerable<InstanceFact<TFactValue>> ToInstanceFacts(
        IEnumerable<Fact<TFactValue>> facts)
    {
        return facts.Select(ToInstanceFact);
    }

    public IEnumerable<InstanceFact<TFactValue>> GetAbstractFactVariants(
        InstanceFactId abstractFactId)
    {
        return GetInRootAndChildren((provider) =>
        {
            var variants = provider
                .modelWrapper
                .VariantsByAbstractFactIds[abstractFactId.FactId];
            return provider.ToInstanceFacts(variants);
        }, abstractFactId.CausalInstanceId);
        //return modelWrapper.FactsAndVariants[abstractFactId.FactId];
    }

    public IEnumerable<InstanceFact<TFactValue>>? TryGetConsequences(
        InstanceFactId causeId)
    {
        return TryGetInRootAndChildren((provider) =>
        {
            provider
                .modelWrapper
                .ConsequencesByCauseIds
                .TryGetValue(causeId.FactId, out var consequences);

            if (consequences == null)
                return null;
            else
                return provider.ToInstanceFacts(consequences);
        }, causeId.CausalInstanceId);
    }
    
    public IEnumerable<InstanceFact<TFactValue>> GetRootCauses()
    {
        return resolvedBlocksByInstanceId
            .Value
            .Values
            .SelectMany(provider =>
                provider.ToInstanceFacts(provider.modelWrapper.RootCauses))
            .Concat(ToInstanceFacts(modelWrapper.RootCauses));
    }
}
