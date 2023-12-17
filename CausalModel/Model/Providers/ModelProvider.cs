using CausalModel.Model.Instance;
using CausalModel.Model.ResolvingModelProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Providers;

public class ModelProvider<TFactValue> : IModelProvider<TFactValue>
{
    private readonly IResolvedModelProvider<TFactValue> resolvedModelProvider;
    private readonly string modelInstanceId;

    public ModelProvider(IResolvedModelProvider<TFactValue> modelProvider,
        string modelInstanceId)
    {
        this.resolvedModelProvider = modelProvider;
        this.modelInstanceId = modelInstanceId;
    }

    public IEnumerable<InstanceFact<TFactValue>> GetInstanceFacts()
    {
        return resolvedModelProvider.GetInstanceFacts(modelInstanceId);
    }

    public InstanceFact<TFactValue> GetModelFact(string factId)
    {
        return resolvedModelProvider.GetFact(new InstanceFactId(factId,
            modelInstanceId));
    }

    //public IEnumerable<InstanceFact<TFactValue>> TryGetInstanceBlocksConsequences()
    //{
    //    return resolvedModelProvider.TryGetInstanceBlocksConsequences(modelInstanceId);
    //}

    public IEnumerable<InstanceFact<TFactValue>> GetExternalCauses()
    {
        return resolvedModelProvider.GetExternalCauses(modelInstanceId);
    }
}
