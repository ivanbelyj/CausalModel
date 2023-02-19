using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.FactCollection
{
    public class FactCollection<TNodeValue> : IFactProvider<TNodeValue>
    {
        private Dictionary<Guid, Fact<TNodeValue>> facts =
            new Dictionary<Guid, Fact<TNodeValue>>();
        public FactCollection() { }
        public FactCollection(IEnumerable<Fact<TNodeValue>> facts)
        {
            foreach (var fact in facts)
            {
                this.facts.Add(fact.Id, fact);
            }
        }
        public Fact<TNodeValue> GetFactById(Guid id) => facts[id];
        
        public IEnumerable<Fact<TNodeValue>> Nodes => facts.Values;
    }
}
