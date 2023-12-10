using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;
public class CausalModelWrapper<TFactValue>
{
    public Dictionary<string, Fact<TFactValue>> FactsById { get; private set; } = new();
    public Dictionary<string, BlockFact> BlocksById { get; private set; } = new();

    public IEnumerable<BlockFact> Blocks => BlocksById.Values;

    public Dictionary<Fact<TFactValue>, List<Fact<TFactValue>>> CausesAndConsequences
        { get; private set; } = new();
    public Dictionary<Fact<TFactValue>, List<Fact<TFactValue>>> FactsAndVariants
        { get; private set; } = new();
    public HashSet<Fact<TFactValue>> RootCauses
        { get; private set; } = new();

    public CausalModelWrapper(CausalModel<TFactValue> causalModel)
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
                if (!FactsAndVariants.ContainsKey(abstractFact))
                {
                    FactsAndVariants.Add(abstractFact,
                        new List<Fact<TFactValue>> { fact });
                }
                else
                {
                    FactsAndVariants[abstractFact].Add(fact);
                }
            }

            foreach (var edge in fact.GetCauses())
            {
                if (edge.CauseId != null)
                {
                    var cause = GetFact(edge.CauseId);
                    if (!CausesAndConsequences.ContainsKey(cause))
                    {
                        CausesAndConsequences.Add(cause,
                            new List<Fact<TFactValue>>() { fact });
                    }
                    else
                    {
                        CausesAndConsequences[cause].Add(fact);
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
