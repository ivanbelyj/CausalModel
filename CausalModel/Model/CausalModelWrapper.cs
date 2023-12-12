using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;

/// <summary>
/// Provides useful data from the causal model in more optimized way
/// </summary>
public class CausalModelWrapper<TFactValue>
{
    public Dictionary<string, Fact<TFactValue>> FactsById { get; private set; } = new();
    public Dictionary<string, BlockFact> BlocksById { get; private set; } = new();

    public IEnumerable<BlockFact> Blocks => BlocksById.Values;

    public Dictionary<string, List<Fact<TFactValue>>> ConsequencesByCauseIds
        { get; private set; } = new();
    public Dictionary<string, List<Fact<TFactValue>>> VariantsByAbstractFactIds
        { get; private set; } = new();
    public HashSet<Fact<TFactValue>> RootCauses
        { get; private set; } = new();

    private readonly CausalModel<TFactValue> model;

    public CausalModelWrapper(CausalModel<TFactValue> causalModel)
    {
        model = causalModel;

        Initialize(model);
    }

    private void Initialize(CausalModel<TFactValue> causalModel)
    {
        foreach (var fact in causalModel.Facts)
        {
            FactsById.Add(fact.Id, fact);
        }

        foreach (var blockFact in causalModel.BlockFacts)
        {
            BlocksById.Add(blockFact.Id, blockFact);
        }

        foreach (Fact<TFactValue> fact in causalModel.Facts)
        {
            if (fact.IsRootCause())
                RootCauses.Add(fact);

            if (fact.AbstractFactId != null)
            {
                var abstractFact = GetFact(fact.AbstractFactId);
                if (!VariantsByAbstractFactIds.ContainsKey(abstractFact.Id))
                {
                    VariantsByAbstractFactIds.Add(abstractFact.Id,
                        new List<Fact<TFactValue>> { fact });
                }
                else
                {
                    VariantsByAbstractFactIds[abstractFact.Id].Add(fact);
                }
            }

            foreach (var edge in fact.GetCauses())
            {
                if (edge.CauseId != null)
                {
                    var cause = GetFact(edge.CauseId);
                    if (!ConsequencesByCauseIds.ContainsKey(cause.Id))
                    {
                        ConsequencesByCauseIds.Add(cause.Id,
                            new List<Fact<TFactValue>>() { fact });
                    }
                    else
                    {
                        ConsequencesByCauseIds[cause.Id].Add(fact);
                    }
                }
            }
        }
    }

    public Fact<TFactValue>? TryGetFact(string id)
    {
        FactsById.TryGetValue(id, out var fact);
        return fact;
    }

    public Fact<TFactValue> GetFact(string id)
    {
        return FactsById[id];
    }
}
