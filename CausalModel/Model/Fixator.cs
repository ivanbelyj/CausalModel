using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;
public class Fixator<TNodeValue> : IFixator<TNodeValue>
{
    private Dictionary<Guid, bool> factsFixated = new Dictionary<Guid, bool>();

    public event FactFixatedEventHandler<TNodeValue>? FactFixated;

    public bool? IsFixated(Guid factId)
        => factsFixated.ContainsKey(factId) ? factsFixated[factId] : null;

    public void FixateFact(Fact<TNodeValue> fact, bool isHappened)
    {
        factsFixated[fact.Id] = isHappened;
        FactFixated?.Invoke(this, fact, isHappened);
    }
}
