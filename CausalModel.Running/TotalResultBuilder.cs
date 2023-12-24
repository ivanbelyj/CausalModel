using CausalModel.Running.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running;
public class TotalResultBuilder
{
    private readonly List<long> simulationTimesMs = new();

    private readonly Dictionary<string, Dictionary<string, int>> modelFactCounts = new();
    private readonly Dictionary<string, int> simulationsCountByModelName = new();

    public void AddSimulationResult(SimulationResult simulationResult)
    {
        simulationTimesMs.Add(simulationResult.ElapsedMilliseconds);

        foreach (var (modelName, countsByFactId) in simulationResult
            .ModelFactCounts)
        {
            if (!modelFactCounts.ContainsKey(modelName))
                modelFactCounts.Add(modelName, new Dictionary<string, int>());

            foreach (var (factId, factCount) in countsByFactId)
            {
                if (!modelFactCounts[modelName].ContainsKey(factId))
                    modelFactCounts[modelName].Add(factId, 0);
                modelFactCounts[modelName][factId] += factCount;
            }
        }

        foreach (var (modelName, simulationsCount) in simulationResult
            .SimulationsCountByModelName)
        {
            if (!simulationsCountByModelName.ContainsKey(modelName))
                simulationsCountByModelName.Add(modelName, 0);

            simulationsCountByModelName[modelName] += simulationsCount;
        }
    }

    private Dictionary<string, Dictionary<string, float>>
        CalculateFactActualProbabilitiesByModelName()
    {
        var res = new Dictionary<string, Dictionary<string, float>>();

        foreach (var (modelName, factCountsByFactId) in modelFactCounts)
        {
            var actualProbabilitiesByFactId = new Dictionary<string, float>();
            res.Add(modelName, actualProbabilitiesByFactId);

            foreach (var (factId, factCount) in factCountsByFactId)
            {
                float actualProbability = (float)factCount
                    / simulationsCountByModelName[modelName];
                actualProbabilitiesByFactId.Add(factId, actualProbability);
            }
        }

        return res;
    }

    public TotalResult Build()
    {
        return new TotalResult()
        {
            MinTimeMilliseconds = simulationTimesMs.Min(),
            MaxTimeMilliseconds = simulationTimesMs.Max(),
            AverageTimeMilliseconds = simulationTimesMs.Average(),
            FactActualProbabilitiesByModelName
                = CalculateFactActualProbabilitiesByModelName(),
        };
    }
}
