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

    public HashSet<string> ExternalCauseIds { get; private set; } = new();

    public CachingDecorator(CausalModel<TFactValue> model)
    {
        this.model = model;

        Initialize(this.model);
    }

    private void Initialize(CausalModel<TFactValue> model)
    {
        // Facts dictionary
        foreach (Fact<TFactValue> fact in model.Facts)
        {
            FactsById.Add(fact.Id, fact);
        }

        // External causes
        foreach (string? cause in model
            .Facts
            .SelectMany(fact => fact.GetCauses())
            .Select(factor => factor.CauseId))
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
