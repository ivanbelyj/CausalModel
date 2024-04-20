using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Fixation.Fixators.Pending
{
    public class DefaultPendingFilter<TFactValue> : IPendingFixationFilter<TFactValue>
        where TFactValue : class
    {
        public bool ShouldBePending(InstanceFact<TFactValue> fact) => true;
    }
}
