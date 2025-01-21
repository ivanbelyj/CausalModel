using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Common.DataProviders
{
    public interface IFactsProvider<TFactValue>
        where TFactValue : class
    {
        Fact<TFactValue> GetFact(string modelName, string factLocalId);
    }
}
