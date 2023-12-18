using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;

/// <summary>
/// Provides facts available as a part of the single defined causal model instance.
/// The code using this interface can be agnostic about what the model it is.
/// Facts available in the model include <i>external facts</i> - consequences of
/// blocks directly declared in the model and causes that parent model provides
/// </summary>
public interface IModelProvider<TFactValue>
{
    /// <summary>
    /// Gets available fact by id (including external facts or direct blocks
    /// consequences)
    /// </summary>
    InstanceFact<TFactValue> GetModelFact(string factId);

    /// <summary>
    /// Gets <i>direct facts</i> of the single model instance.
    /// They not including neither direct blocks consequences,
    /// nor facts encapsulated in the blocks,
    /// nor external required causes
    /// </summary>
    IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts();
}
