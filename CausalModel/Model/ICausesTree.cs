using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model
{
    /// <summary>
    /// Providing data that allows to traverse from causes to consequences
    /// (and also information about abstract facts and their implementations)
    /// </summary>
    public interface ICausesTree<TFactValue>
        where TFactValue : class
    {
        IEnumerable<InstanceFact<TFactValue>> GetAbstractFactVariants(
            InstanceFactId abstractFactId);
        IEnumerable<InstanceFact<TFactValue>>? TryGetConsequences(InstanceFactId id);
    }
}
