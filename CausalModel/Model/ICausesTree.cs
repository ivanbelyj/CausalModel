using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;
public interface ICausesTree<TFactValue>
{
    IEnumerable<InstanceFact<TFactValue>> GetAbstractFactVariants(
        InstanceFactId abstractFactId);
    IEnumerable<InstanceFact<TFactValue>>? TryGetConsequences(InstanceFactId id);
}
