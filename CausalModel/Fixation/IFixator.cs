using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation;

public delegate void FactFixatedEventHandler<TFactValue>(object sender,
    Fact<TFactValue> factFixated, bool isHappened);

public interface IFixator<TFactValue> : IFixatedProvider
{
    event FactFixatedEventHandler<TFactValue> FactFixated;
    void FixateFact(Fact<TFactValue> fact, bool isHappened);
}
