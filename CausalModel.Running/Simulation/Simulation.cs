using CausalModel.Fixation;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.Simulation;
public class Simulation<TFactValue>
{
    private readonly FixationFacade<TFactValue> fixation;
    private CausalGenerator<TFactValue>? generator;
    private readonly SimulationResultBuilder<TFactValue> resultBuilder;

    public Simulation(FixationFacade<TFactValue> fixation)
    {
        this.fixation = fixation;
        resultBuilder = new();

        fixation.ModelInstanceFactory.ModelInstantiated += OnModelInstantiated;
        fixation.Fixator.FactFixated += OnFactFixated;
    }

    private void OnModelInstantiated(object sender,
        ModelInstance<TFactValue> modelInstance)
    {
        resultBuilder.AddModelInstantiated(modelInstance);
    }

    private void OnFactFixated(
        object sender,
        InstanceFactId fixatedFactId,
        bool isHappened)
    {
        var fact = generator!.ModelProvider.GetFact(fixatedFactId);
        resultBuilder.AddFact(fact, isHappened);
    }

    public SimulationResult Run(int? seed = null)
    {
        generator = fixation.CreateGenerator(seed);

        Stopwatch stopwatch = Stopwatch.StartNew();
        generator.FixateRootCauses();
        stopwatch.Stop();

        var res = resultBuilder
            .Build(stopwatch.ElapsedMilliseconds);

        return res;
    }
}
