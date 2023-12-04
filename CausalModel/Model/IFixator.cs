using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;

public delegate void FactFixatedEventHandler<TNodeValue>(object sender,
    Fact<TNodeValue> factFixated, bool isHappened);

public interface IFixator<TNodeValue> : IFixatedProvider
{
    event FactFixatedEventHandler<TNodeValue> FactFixated;
    void FixateFact(Fact<TNodeValue> fact, bool isHappened);
}
