//using CausalModel.Blocks.Resolving;
//using CausalModel.Facts;
//using CausalModel.Model.Instance;
//using CausalModel.Model.Providers;
//using System.Linq;

//namespace CausalModel.Model.ResolvingModelProvider;

///// <summary>
///// Resolving blocks and provides causal model data
///// </summary>
//public class ResolvingModelProviderOld<TFactValue> : IResolvedModelProvider<TFactValue>
//{
//    private readonly IBlockResolver<TFactValue> resolver;
//    private readonly Lazy<Dictionary<string, ResolvedModelProvider<TFactValue>>>
//        resolvedBlocksByInstanceId;

//    private readonly ModelInstance<TFactValue> rootInstance;
//    private readonly CachingDecorator<TFactValue> rootModelDecorator;

//    private readonly CausesTree<TFactValue> causesTree;

//    private readonly ResolvedModelProvider<TFactValue>? parent;

//    public ResolvingModelProviderOld(ModelInstance<TFactValue> causalInstance,
//        IBlockResolver<TFactValue> blockResolver,
//        CausesTree<TFactValue>? causesTree = null)
//    {
//        resolver = blockResolver;
//        resolvedBlocksByInstanceId = new(GetResolvedBlocks);

//        rootInstance = causalInstance;
//        rootModelDecorator = new CachingDecorator<TFactValue>(causalInstance.Model);

//        parent = null;
//        this.causesTree = causesTree ?? new();
//    }

//    private ResolvingModelProviderOld(
//        ModelInstance<TFactValue> causalInstance,
//        IBlockResolver<TFactValue> blockResolver,
//        ResolvedModelProvider<TFactValue> parent)
//        : this(causalInstance, blockResolver, parent.causesTree)
//    {
//        this.parent = parent;
//        causesTree.AddModel(new ModelProvider<TFactValue>(
//            this, rootInstance.InstanceId));
//    }

//    public string RootModelInstanceId => rootInstance.InstanceId;

//    /// <summary>
//    /// Returns a dictionary of resolved blocks of the causal model
//    /// (not recursive resolving)
//    /// </summary>
//    /// <returns>Resolving model providers by ids</returns>
//    private Dictionary<string, ResolvedModelProvider<TFactValue>>
//        GetResolvedBlocks()
//    {
//        var resolvedBlocks
//            = new Dictionary<string, ResolvedModelProvider<TFactValue>>();

//        foreach (BlockFact block in rootInstance.Model.BlockFacts)
//        {
//            ModelInstance<TFactValue> resolvedBlock =
//                resolver.Resolve(block.Block, rootInstance);

//            resolvedBlocks.Add(resolvedBlock.InstanceId,
//                new ResolvingModelProvider<TFactValue>(resolvedBlock, resolver,
//                    this));
//        }
//        return resolvedBlocks;
//    }

//    public InstanceFact<TFactValue> GetFact(InstanceFactId id)
//    {
//        var res = TryGetFact(id);
//        if (res == null)
//            throw new InvalidOperationException($"Fact (id: {id}) was not found.");
//        return res;
//    }

//    public InstanceFact<TFactValue>? TryGetFactInRootInstance(string factLocalId)
//    {
//        var fact = rootModelDecorator.TryGetFact(factLocalId);
//        return fact == null ? null : ToInstanceFact(fact);
//    }
//    public InstanceFact<TFactValue> GetFactInRootInstance(string factLocalId)
//    {
//        var res = TryGetFactInRootInstance(factLocalId);
//        if (res == null)
//            throw new InvalidOperationException($"Fact (id: {factLocalId}) " +
//                $"was not found in the root model (instance id: " +
//                $"{RootModelInstanceId})");
//        return res;
//    }

//    private ResolvedModelProvider<TFactValue> GetInstanceProvider(
//        string modelInstanceId)
//    {
//        var res = TryGetInstanceProvider(modelInstanceId);
//        if (res == null)
//            throw new InvalidOperationException($"Required causal model instance"
//                + $" (id: {modelInstanceId}) was not found.");

//        return res;
//    }

//    private ResolvedModelProvider<TFactValue>? TryGetInstanceProvider(
//        string modelInstanceId)
//    {
//        if (modelInstanceId == rootInstance.InstanceId)
//            return this;
//        else
//        {
//            resolvedBlocksByInstanceId.Value.TryGetValue(modelInstanceId,
//                out var res);
//            return res;
//        }
//    }

//    private bool IsRootModelDependency(InstanceFactId id)
//    {
//        // Todo: optimize ?
//        return rootInstance
//            .Model
//            .BlockConventions
//            ?.SelectMany(x => x.Consequences ?? new List<BaseFact>())
//            .Select(x => x.Id)
//            .Contains(id.FactId) ?? false;
//    }

//    public InstanceFact<TFactValue>? TryGetFact(InstanceFactId id)
//    {
//        var provider = TryGetInstanceProvider(id.CausalInstanceId);
//        if (provider == null)
//            return null;

//        // Model dependencies (consequences of the declared blocks)
//        // are also considered as a part of the model instance
//        // (these facts are aslo adressed by the same model instance id)
//        // but actually they are located in the children providers
//        if (IsRootModelDependency(id))
//        {
//            // Todo: optimize ?
//            foreach (var blockProvider in resolvedBlocksByInstanceId.Value
//                .Values)
//            {
//                var instanceFact = blockProvider
//                    .TryGetFactInRootInstance(id.FactId);

//                if (instanceFact != null)
//                    return instanceFact;
//            }
//        }

//        return provider.TryGetFactInRootInstance(id.FactId);
//    }

//    private InstanceFact<TFactValue> ToInstanceFact(Fact<TFactValue> fact)
//    {
//        return new InstanceFact<TFactValue>(fact, rootInstance.InstanceId);
//    }

//    private IEnumerable<InstanceFact<TFactValue>> ToInstanceFacts(
//        IEnumerable<Fact<TFactValue>> facts)
//    {
//        return facts.Select(ToInstanceFact);
//    }

//    public IEnumerable<InstanceFact<TFactValue>> GetAbstractFactVariants(
//        InstanceFactId abstractFactId)
//    {
//        //var provider = GetInstanceProvider(abstractFactId.CausalInstanceId);
//        var variants = causesTree
//            .VariantsByAbstractFactIds[abstractFactId];
//        return variants;
//    }

//    public IEnumerable<InstanceFact<TFactValue>>? TryGetConsequences(
//        InstanceFactId causeId)
//    {
//        //var provider = TryGetInstanceProvider(causeId.CausalInstanceId);
//        //if (provider == null)
//        //    return null;

//        //provider
//        //    .rootModelDecorator
//        //    .ConsequencesByCauseIds
//        //    .TryGetValue(causeId.FactId, out var consequences);

//        //if (consequences == null)
//        //    return null;
//        //else
//        //    return provider.ToInstanceFacts(consequences);

//        causesTree
//            .ConsequencesByCauseIds
//            .TryGetValue(causeId, out var consequences);

//        return consequences;
//    }

//    public IEnumerable<InstanceFact<TFactValue>> GetRootFacts()
//    {
//        return resolvedBlocksByInstanceId
//            .Value
//            .Values
//            .SelectMany(provider =>
//                provider.ToInstanceFacts(provider.rootModelDecorator.RootFacts))
//            .Concat(ToInstanceFacts(rootModelDecorator.RootFacts));
//    }

//    public IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts(string modelInstanceId)
//    {
//        var modelInstance = GetInstanceProvider(modelInstanceId);
//        var modelFacts = modelInstance
//            .ToInstanceFacts(modelInstance.rootInstance.Model.Facts);

//        return modelFacts;
//    }

//    //public IEnumerable<InstanceFact<TFactValue>> TryGetInstanceBlocksConsequences(
//    //    string modelInstanceId)
//    //{
//    //    var modelInstance = GetInstanceProvider(modelInstanceId);
//    //    var consequences = modelInstance
//    //        .rootInstance
//    //        .Model
//    //        .BlockConventions
//    //        ?.SelectMany(conv => conv
//    //            .Consequences?
//    //            .Select(consequence => modelInstance.GetFact(
//    //                new InstanceFactId(consequence.Id, modelInstanceId)))
//    //            ?? new List<InstanceFact<TFactValue>>())
//    //        ?? new List<InstanceFact<TFactValue>>();
//    //    return consequences;
//    //}

//    public InstanceFact<TFactValue> GetExternalCause(
//        string modelInstanceId,
//        string causeId)
//    {
//        var provider = GetInstanceProvider(modelInstanceId);

//        if (provider.parent == null)
//            throw new InvalidOperationException("Provider without parent "
//                + "cannot provide external causes.");

//        return provider.parent.GetFactInRootInstance(causeId);
//    }

//    public IEnumerable<InstanceFact<TFactValue>> GetExternalCauses(
//        string modelInstanceId)
//    {
//        var provider = GetInstanceProvider(modelInstanceId);

//        if (provider.parent == null)
//            throw new InvalidOperationException("Provider without parent "
//                + "cannot provide external causes.");

//        return provider
//            .rootModelDecorator
//            .ExternalCauseIds
//            .Select(provider.parent.GetFactInRootInstance);
//    }
//}
