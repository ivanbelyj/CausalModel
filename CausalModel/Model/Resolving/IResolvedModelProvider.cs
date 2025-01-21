using CausalModel.Facts;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving.ResolvingNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{
    /// <summary>
    /// Provides resolved causal model data.
    /// Resolved model includes all resolved blocks
    /// (resolved blocks include their resolved blocks, etc.)
    /// </summary>
    public interface IResolvedModelProvider<TFactValue>
        where TFactValue : class
    {
        InstanceFact<TFactValue> GetFact(InstanceFactAddress address);
        InstanceFact<TFactValue>? TryGetFact(InstanceFactAddress address);
        IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts(string modelInstanceId);
        IEnumerable<InstanceFact<TFactValue>> GetResolvedFacts();
        IModelProvider<TFactValue> GetModelProvider(string modeInstanceId);
        string RootInstanceId { get; }
        IModelProvider<TFactValue> GetRootModelProvider()
        {
            return GetModelProvider(RootInstanceId);
        }
    }
}
