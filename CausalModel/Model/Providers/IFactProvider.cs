using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Providers
{
    public interface IFactProvider<TFactValue>
    {
        Fact<TFactValue> GetFact(string id);
        Fact<TFactValue>? TryGetFact(string id);
    }
}
