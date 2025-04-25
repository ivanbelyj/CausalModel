using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.Models.Common
{
    public class FactInfo<TFactValue>
        where TFactValue : class
    {
        public string FactLocalId { get; set; }
        public TFactValue? FactValue { get; set; }
        public int FactCount { get; set; }

        public FactInfo(string factLocalId, TFactValue? factValue)
        {
            FactLocalId = factLocalId;
            FactValue = factValue;
        }

        public FactInfo(FactInfo<TFactValue> factInfo) : this(factInfo.FactLocalId, factInfo.FactValue)
        {
            
        }
    }
}
