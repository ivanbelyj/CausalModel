using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Providers;

/// <summary>
/// Provides facts from some defined causal model instance.
/// The code using this interface can be agnostic about what the model it is
/// </summary>
public interface IFactProvider<TFactValue>
{
    InstanceFact<TFactValue> GetInstanceFact(string factId);
}
