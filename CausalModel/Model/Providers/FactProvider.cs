using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Providers;
public class FactProvider<TFactValue> : IFactProvider<TFactValue>
{
    private readonly IInstanceFactProvider<TFactValue> instanceFactProvider;
    private readonly string modelInstanceId;

    public FactProvider(IInstanceFactProvider<TFactValue> instanceFactProvider,
        string modelInstanceId)
    {
        this.instanceFactProvider = instanceFactProvider;
        this.modelInstanceId = modelInstanceId;
    }

    public InstanceFact<TFactValue> GetInstanceFact(string factId)
    {
        return instanceFactProvider.GetFact(new InstanceFactId(factId,
            modelInstanceId));
    }
}
