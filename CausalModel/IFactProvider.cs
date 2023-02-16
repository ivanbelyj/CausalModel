using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel
{
    public interface IFactProvider<TNodeValue>
    {
        public Fact<TNodeValue> GetFactById(Guid id);
    }
}
