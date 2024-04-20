using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Fixation.Fixators.Pending
{
    public interface IPendingFixationFilter<TFactValue> where TFactValue : class
    {
        bool ShouldBePending(InstanceFact<TFactValue> fact);
    }
}
