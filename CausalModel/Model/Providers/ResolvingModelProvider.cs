using CausalModel.Facts;
using CausalModel.Model.Blocks;

namespace CausalModel.Model.Providers;
public class ResolvingModelProvider<TFactValue> : IModelProvider<TFactValue>
{
    private readonly BlockResolver<TFactValue> resolver;
    private List<ResolvingModelProvider<TFactValue>>? resolvedBlocks;

    private CausalModelWrapper<TFactValue> modelWrapper;

    public ResolvingModelProvider(CausalModel<TFactValue> causalModel,
        BlockResolver<TFactValue> blockResolver)
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
                Resolve();

            foreach (var resolvedBlock in resolvedBlocks!)
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

    /// <summary>
    /// Resolves blocks in the causal model (not recursively)
    /// </summary>
    public void Resolve()
    {
        resolvedBlocks = new List<ResolvingModelProvider<TFactValue>>();
        foreach (BlockFact block in modelWrapper.Blocks)
        {
            ResolvingModelProvider<TFactValue> resolvedBlock = resolver.Resolve(block);
            resolvedBlocks.Add(resolvedBlock);
        }
    }

    public IEnumerable<Fact<TFactValue>> GetAbstractFactVariants(
        Fact<TFactValue> abstractFact)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Fact<TFactValue>> GetConsequences(Fact<TFactValue> fact)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Fact<TFactValue>> GetRootCauses()
    {
        throw new NotImplementedException();
    }
}
