using CausalModel.Fixation;
using CausalModel.Running.Simulation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running;
public class SimulationsRunner<TFactValue>
{
    private readonly IFixationFacadeFactory<TFactValue> generatorFactory;
    private readonly Random random = new();

    public SimulationsRunner(IFixationFacadeFactory<TFactValue> generatorFactory)
	{
        this.generatorFactory = generatorFactory;
    }

    public TotalResult RunSimulations(int count = 1000)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        TotalResultBuilder totalResBuilder = new();

        for (int i = 0; i < count; i++)
        {
            var simRes = RunSimulation();
            totalResBuilder.AddSimulationResult(simRes);
        }

        stopwatch.Stop();

        var res = totalResBuilder.Build();
        res.TotalMilliseconds = stopwatch.ElapsedMilliseconds;
        return res;
    }

    public SimulationResult RunSimulation()
    {
        var fixationFacade = generatorFactory.Create();
        Simulation<TFactValue> simulation = new(fixationFacade);
        return simulation.Run(random.Next());
    }
}
