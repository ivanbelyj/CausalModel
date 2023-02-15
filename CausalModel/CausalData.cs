using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel
{
    public class CausalData<TNodeValue>
    {
        private Dictionary<Guid, Fact<TNodeValue>> facts;
        public CausalData()
        {
            facts = new Dictionary<Guid, Fact<TNodeValue>>();
        }
        public Fact<TNodeValue> GetFactById(Guid id) => facts[id];
        public IEnumerable<Fact<TNodeValue>> Nodes => facts.Values;
    }
}
