using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation;

public delegate void FactFixatedEventHandler<TFactValue>(object sender,
    Fact<TFactValue> factFixated, bool isHappened);

/// <summary>
/// A component of the causal model fixation process
/// responsible for fixating the facts and related features
/// (storing a state, override fixation data, etc.)
/// </summary>
/// <typeparam name="TFactValue">The type of the causal model fact value</typeparam>
public interface IFixator<TFactValue> : IFixatedProvider
{
    event FactFixatedEventHandler<TFactValue> FactFixated;
    void FixateFact(Fact<TFactValue> fact, bool isHappened);
}
