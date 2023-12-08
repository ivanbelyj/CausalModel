using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation;
public class Fixator<TNodeValue> : IFixator<TNodeValue>
{
    private Dictionary<string, bool> factsFixated = new Dictionary<string, bool>();

    public event FactFixatedEventHandler<TNodeValue>? FactFixated;

    public bool? IsFixated(string factId)
        => factsFixated.ContainsKey(factId) ? factsFixated[factId] : null;

    public virtual void FixateFact(Fact<TNodeValue> fact, bool isHappened)
    {
        factsFixated[fact.Id] = isHappened;
        FactFixated?.Invoke(this, fact, isHappened);
    }
}