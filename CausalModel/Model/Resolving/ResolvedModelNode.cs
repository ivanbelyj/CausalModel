using CausalModel.Blocks.Resolving;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving;
internal class ResolvedModelNode<TFactValue>
{
    private readonly ModelInstance<TFactValue> modelInstance;
    private readonly IBlockResolver<TFactValue> resolver;
    private readonly CachingDecorator<TFactValue> cachingDecorator;

    private readonly IEnumerable<ResolvedModelNode<TFactValue>>? blocks;
    private readonly ResolvedModelNode<TFactValue>? parent;

    public ResolvedModelNode(
        ModelInstance<TFactValue> modelInstance,
        IBlockResolver<TFactValue> resolver,
        IEnumerable<ResolvedModelNode<TFactValue>>? blocks,
        ResolvedModelNode<TFactValue>? parent = null)
	{
        this.modelInstance = modelInstance;
        this.resolver = resolver;
        this.blocks = blocks;
        this.parent = parent;

        cachingDecorator = new CachingDecorator<TFactValue>(modelInstance.Model);
    }

    public string ModelInstanceId => modelInstance.InstanceId;

    public bool IsOwn(string modelInstanceId)
    {
        return modelInstance.InstanceId == modelInstanceId;
    }

    /// <summary>
    /// Returns resolved blocks of the root causal model instance
    /// (not recursive resolving)
    /// </summary>
    public IEnumerable<ModelInstance<TFactValue>> GetResolvedBlocks()
    {
        var res = new List<ModelInstance<TFactValue>>();
        foreach (BlockFact block in modelInstance.Model.BlockFacts)
        {
            ModelInstance<TFactValue> resolvedBlock =
                resolver.Resolve(block.Block, modelInstance);

            res.Add(resolvedBlock);
        }
        return res;
    }

    public InstanceFact<TFactValue>? TryGetFact(string factLocalId)
    {
        var fact = cachingDecorator.TryGetFact(factLocalId);
        return fact == null ? null : ToInstanceFact(fact);
    }

    public InstanceFact<TFactValue> GetFact(string factLocalId)
    {
        var res = TryGetFact(factLocalId);
        if (res == null)
            throw new InvalidOperationException($"Fact (local id: {factLocalId}) "
                + $"was not found in the model instance "
                + $"(id: {modelInstance.InstanceId}).");
        return res;
    }

    private InstanceFact<TFactValue> ToInstanceFact(Fact<TFactValue> fact)
    {
        return new InstanceFact<TFactValue>(fact, modelInstance.InstanceId);
    }
    private IEnumerable<InstanceFact<TFactValue>> ToInstanceFacts(
        IEnumerable<Fact<TFactValue>> facts)
    {
        return facts.Select(ToInstanceFact);
    }

    public IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts()
    {
        var modelFacts = ToInstanceFacts(modelInstance.Model.Facts);
        return modelFacts;
    }

    public IEnumerable<InstanceFact<TFactValue>>? TryGetExternalCauses()
    {
        //if (parent == null)
        //    throw new InvalidOperationException("Provider without parent "
        //        + "cannot provide external causes.");

        if (parent == null)
            return null;

        IEnumerable<InstanceFact<TFactValue>> factsFromParent = cachingDecorator
            .ExternalCauseIds
            .Select(parent.TryGetFact)
            .Where(parentFact => parentFact != null)!;

        //var blockConsequenceIds = modelInstance
        //    .Model
        //    .BlockConventions
        //    ?.SelectMany(x => x.Consequences ?? new List<BaseFact>())
        //    .Select(x => x.Id);

        List<InstanceFact<TFactValue>> factsFromBlocks = new();
        if (blocks != null)
        {
            foreach (var block in blocks)
            {
                factsFromBlocks.AddRange(cachingDecorator
                    .ExternalCauseIds
                    .SelectMany(externalId =>
                        blocks.Select(block => block.TryGetFact(externalId)))
                    .Where(fact => fact != null)!);
            }
        }

        return factsFromParent.Concat(factsFromBlocks);
    }
}
