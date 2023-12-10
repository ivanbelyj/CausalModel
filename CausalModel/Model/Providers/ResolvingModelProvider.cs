using CausalModel.Facts;
using CausalModel.Model.Blocks;

namespace CausalModel.Model.Providers;
public class ResolvingModelProvider<TFactValue> : IModelProvider<TFactValue>
{
    private readonly BlockImplementationResolver<TFactValue> resolver;
    private List<ResolvingModelProvider<TFactValue>>? resolvedBlocks;

    private CausalModelWrapper<TFactValue> modelWrapper;
    //private CausalModel<TFactValue> model;

    public ResolvingModelProvider(CausalModel<TFactValue> causalModel,
        BlockImplementationResolver<TFactValue> blockResolver)
    {
        modelWrapper = new CausalModelWrapper<TFactValue>(causalModel);
        resolver = blockResolver;
    }

    private T? Traverse<T>(Func<ResolvingModelProvider<TFactValue>, T?> func)
        where T : class
    {
        T? res = func(this);

        if (res != null)
            return res;
        else
        {
            // Todo: should resolve here?
            if (resolvedBlocks == null)
                resolvedBlocks = GetResolved();

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

    public Fact<TFactValue>? TryGetFact(string id)
    {
        return Traverse((provider) =>
        {
            return provider.modelWrapper.TryGetFact(id);
        });
    }

    private Fact<TFactValue> Resolve(Fact<TFactValue> fact)
        => GetFact(fact.Id);

    /// <summary>
    /// Returns resolved blocks of the causal model (not recursive resolving)
    /// </summary>
    public List<ResolvingModelProvider<TFactValue>> GetResolved()
    {
        var resolvedBlocks = new List<ResolvingModelProvider<TFactValue>>();
        foreach (BlockFact block in modelWrapper.Blocks)
        {
            ResolvingModelProvider<TFactValue> resolvedBlock = resolver.Resolve(block);
            resolvedBlocks.Add(resolvedBlock);
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
        modelWrapper.CausesAndConsequences.TryGetValue(fact, out var consequences);
        return consequences;
    }

    public IEnumerable<Fact<TFactValue>> GetRootCauses()
    {
        if (resolvedBlocks == null)
            resolvedBlocks = GetResolved();

        return resolvedBlocks
            .SelectMany(provider => provider.GetRootCauses())
            .Concat(modelWrapper.RootCauses.ToList());
    }
}
