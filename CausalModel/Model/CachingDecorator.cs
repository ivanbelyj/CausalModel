using CausalModel.Factors;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;

/// <summary>
/// Caching data from the causal model for faster access
/// </summary>
public class CachingDecorator<TFactValue>
{
    private readonly CausalModel<TFactValue> model;

    public Dictionary<string, Fact<TFactValue>> FactsById
        { get; private set; } = new();
    //public HashSet<Fact<TFactValue>> RootFacts { get; private set; } = new();

    public HashSet<string> ExternalCauseIds { get; private set; } = new();

    //private readonly IModelProvider<TFactValue> modelProvider;

    public CachingDecorator(CausalModel<TFactValue> model)
        //IModelProvider<TFactValue> modelProvider)
    {
        //this.modelProvider = modelProvider;

        this.model = model;

        Initialize(this.model);
    }

    private void Initialize(CausalModel<TFactValue> model)
    {
        //var consequences = (modelProvider.TryGetInstanceBlocksConsequences()
        //    ?? new List<InstanceFact<TFactValue>>());

        // Facts dictionary and roots set
        foreach (Fact<TFactValue> fact in model.Facts)
            //.GetInstanceFacts()
            //.Concat(consequences))
        {
            FactsById.Add(fact.Id, fact);

            //if (fact.IsRootCause())
            //    RootFacts.Add(fact);
        }

        // External causes
        foreach (string? cause in model
            .Facts
            .SelectMany(fact => fact.GetCauses())
            .Select(factor => factor.CauseId))
            //.GetInstanceFacts()
            //.SelectMany(fact => fact.Fact.GetCauses())
            //.Select(factor => factor.CauseId))
        {
            if (cause != null
                && !FactsById.ContainsKey(cause)
                && !ExternalCauseIds.Contains(cause))
                ExternalCauseIds.Add(cause);
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
