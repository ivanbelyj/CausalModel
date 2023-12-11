using CausalModel.Blocks.Resolving;
using CausalModel.Facts;

namespace CausalModel.Model.Providers;

/// <summary>
/// Resolving blocks and provides causal model data
/// </summary>
public class ResolvingModelProvider<TFactValue> : ICausalModelProvider<TFactValue>
{
    private readonly IBlockResolver<TFactValue> resolver;
    private List<ResolvingModelProvider<TFactValue>>? resolvedBlocks;

    private readonly CausalModelWrapper<TFactValue> modelWrapper;

    // Todo: Should use model ?
    private readonly CausalModel<TFactValue> rootModel;

    public ResolvingModelProvider(CausalModel<TFactValue> causalModel,
        IBlockResolver<TFactValue> blockResolver)
    {
        modelWrapper = new CausalModelWrapper<TFactValue>(causalModel);
        this.rootModel = causalModel;
        resolver = blockResolver;
    }

    /// <summary>
    /// Finds something via function in the provider and its blocks (not recursively)
    /// </summary>
    private T? Find<T>(Func<ResolvingModelProvider<TFactValue>, T?> func)
        where T : class
    {
        T? res = func(this);

        if (res != null)
            return res;
        else
        {
            // Todo: should resolve here?
            if (resolvedBlocks == null)
                resolvedBlocks = GetResolvedBlocks();

            foreach (var resolvedBlock in resolvedBlocks)
            {
                var resInBlock = func(resolvedBlock);
                if (resInBlock != null)
                    return resInBlock;
            }
        }

        return null;
    }

    public Fact<TFactValue> GetFact(string id)
    {
        var res = TryGetFact(id);
        if (res == null)
            throw new InvalidOperationException($"Fact (id: {id}) was not found.");
        return res;
    }

    public Fact<TFactValue>? TryGetFactInRootModel(string id)
    {
        return modelWrapper.TryGetFact(id);
    }

    public Fact<TFactValue>? TryGetFact(string id)
    {
        return Find((provider) =>
        {
            return provider.TryGetFactInRootModel(id);
        });
    }

    private Fact<TFactValue> Resolve(Fact<TFactValue> fact)
        => GetFact(fact.Id);

    /// <summary>
    /// Returns resolved blocks of the causal model (not recursive resolving)
    /// </summary>
    public List<ResolvingModelProvider<TFactValue>> GetResolvedBlocks()
    {
        var resolvedBlocks = new List<ResolvingModelProvider<TFactValue>>();
        foreach (BlockFact block in modelWrapper.Blocks)
        {
            CausalModel<TFactValue> resolvedBlock =
                resolver.Resolve(block.Block, rootModel);
            resolvedBlocks.Add(new ResolvingModelProvider<TFactValue>(resolvedBlock,
                resolver));
        }
        return resolvedBlocks;
    }

    public IEnumerable<Fact<TFactValue>> GetAbstractFactVariants(
        Fact<TFactValue> abstractFact)
    {
        return modelWrapper.FactsAndVariants[Resolve(abstractFact)];
    }

    public IEnumerable<Fact<TFactValue>>? TryGetConsequences(Fact<TFactValue> fact)
    {
        modelWrapper.CausesAndConsequences.TryGetValue(Resolve(fact),
            out var consequences);
        return consequences;
    }

    public IEnumerable<Fact<TFactValue>> GetRootCauses()
    {
        if (resolvedBlocks == null)
            resolvedBlocks = GetResolvedBlocks();

        return resolvedBlocks
            .SelectMany(provider => provider.GetRootCauses())
            .Concat(modelWrapper.RootCauses.ToList());
    }
}
