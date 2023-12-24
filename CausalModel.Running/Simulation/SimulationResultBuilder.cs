using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.Simulation;
public class SimulationResultBuilder<TFactValue>
{
    private readonly Dictionary<string, ModelInstance<TFactValue>> modelsByInstanceId
        = new();
    private readonly Dictionary<string, Dictionary<string, int>> fixatedFactCounts
        = new();
    private readonly Dictionary<string, int> simulationsCountByModelName = new();

    public void AddModelInstantiated(ModelInstance<TFactValue> modelInstance)
    {
        string? modelName = modelInstance.Model.Name;
        if (modelName == null)
            throw new ArgumentException($"Causal model name" +
                $" (id: {modelInstance.InstanceId})" +
                $" should be set for Monte-Carlo simulation.");

        if (!modelsByInstanceId.ContainsKey(modelInstance.InstanceId))
            modelsByInstanceId.Add(modelInstance.InstanceId, modelInstance);

        if (!simulationsCountByModelName.ContainsKey(modelName))
            simulationsCountByModelName.Add(modelName, 0);
        simulationsCountByModelName[modelName]++;

        if (!fixatedFactCounts.ContainsKey(modelName))
            fixatedFactCounts.Add(modelName, new());
    }

    public void AddFact(InstanceFact<TFactValue> instanceFact, bool isHappened)
    {
        modelsByInstanceId.TryGetValue(instanceFact.InstanceFactId.ModelInstanceId,
            out var modelInstance);

        if (modelInstance == null)
            throw new ArgumentException($"Invalid model of the instance fact" +
                $" (instance fact id: {instanceFact.InstanceFactId}).");

        // Models added to the dictionary have names
        string modelName = modelInstance?.Model.Name!;

        //if (modelName == null)
        //    throw new ArgumentException($"Invalid model of the instance fact" +
        //        $" (instance fact id: {instanceFact.InstanceFactId}).");

        string factLocalId = instanceFact.Fact.Id;
        
        if (!fixatedFactCounts[modelName].ContainsKey(factLocalId))
            fixatedFactCounts[modelName].Add(factLocalId, 0);

        if (isHappened)
            fixatedFactCounts[modelName][factLocalId]++;
    }

    public SimulationResult Build(long elapsedMilliseconds)
    {
        return new SimulationResult()
        {
            ElapsedMilliseconds = elapsedMilliseconds,
            ModelFactCounts = fixatedFactCounts,
            SimulationsCountByModelName = simulationsCountByModelName,
        };
    }
}
