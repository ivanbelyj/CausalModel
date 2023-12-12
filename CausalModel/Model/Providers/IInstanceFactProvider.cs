using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Providers
{
    public interface IInstanceFactProvider<TFactValue>
    {
        InstanceFact<TFactValue> GetFact(InstanceFactId id);
        InstanceFact<TFactValue>? TryGetFact(InstanceFactId id);
    }
}
