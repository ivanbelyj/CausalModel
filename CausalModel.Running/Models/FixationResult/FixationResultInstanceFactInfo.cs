using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.Models.FixationResult
{
    public class FixationResultInstanceFactInfo<TFactValue>
        where TFactValue : class
    {
        public InstanceFactId InstanceFactId { get; private set; }
        public TFactValue? FactValue { get; private set; }

        private FixationResultInstanceFactInfo(
            InstanceFactId instanceFactId,
            TFactValue? factValue)
        {
            InstanceFactId = instanceFactId;
            FactValue = factValue;
        }

        public static FixationResultInstanceFactInfo<TFactValue> Create(InstanceFact<TFactValue> instanceFact)
        {
            return new FixationResultInstanceFactInfo<TFactValue>(
                instanceFact.InstanceFactId,
                instanceFact.Fact.FactValue);
        }
    }
}
