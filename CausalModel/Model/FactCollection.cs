using CausalModel.Facts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;
public class FactCollection<TNodeValue> : IFactProvider<TNodeValue>,
    IEnumerable<Fact<TNodeValue>>
{
    private Dictionary<string, Fact<TNodeValue>> facts =
        new Dictionary<string, Fact<TNodeValue>>();
    public FactCollection() { }
    public FactCollection(IEnumerable<Fact<TNodeValue>> facts)
    {
        foreach (var fact in facts)
        {
            this.facts.Add(fact.Id, fact);
        }
    }
    public Fact<TNodeValue> GetFactById(string id) => facts[id];

    public IEnumerator<Fact<TNodeValue>> GetEnumerator()
    {
        return facts.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
