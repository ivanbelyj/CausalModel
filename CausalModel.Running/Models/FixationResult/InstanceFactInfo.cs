using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.Models.FixationResult
{
    public class InstanceFactInfo<TFactValue>
        where TFactValue : class
    {
        public InstanceFactId InstanceFactId { get; private set; }
        public TFactValue? FactValue { get; private set; }

        private InstanceFactInfo(
            InstanceFactId instanceFactId,
            TFactValue? factValue)
        {
            InstanceFactId = instanceFactId;
            FactValue = factValue;
        }

        public static InstanceFactInfo<TFactValue> Create(InstanceFact<TFactValue> instanceFact)
        {
            return new InstanceFactInfo<TFactValue>(
                instanceFact.InstanceFactId,
                instanceFact.Fact.FactValue);
        }
    }
}
