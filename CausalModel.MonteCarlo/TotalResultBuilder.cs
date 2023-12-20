using CausalModel.MonteCarlo.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.MonteCarlo;
public class TotalResultBuilder
{
    private List<long> simulationTimesMs = new();

    public void AddSimulationResult(SimulationResult simulationResult)
    {
        simulationTimesMs.Add(simulationResult.ElapsedMilliseconds);
    }

    public TotalResult Build()
    {
        return new TotalResult()
        {
            MinTimeMilliseconds = simulationTimesMs.Min(),
            MaxTimeMilliseconds = simulationTimesMs.Max(),
            AverageTimeMilliseconds = simulationTimesMs.Average(),
        };
    }
}
