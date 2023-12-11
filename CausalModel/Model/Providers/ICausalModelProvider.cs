using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Providers;

/// <summary>
/// Provides some causal model data
/// </summary>
public interface ICausalModelProvider<TFactValue> : IFactProvider<TFactValue>
{
    IEnumerable<Fact<TFactValue>> GetAbstractFactVariants(Fact<TFactValue> abstractFact);
    IEnumerable<Fact<TFactValue>>? TryGetConsequences(Fact<TFactValue> fact);
    IEnumerable<Fact<TFactValue>> GetRootCauses();
}
