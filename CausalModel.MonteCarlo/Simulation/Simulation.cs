using CausalModel.Fixation;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.MonteCarlo.Simulation;
public class Simulation<TFactValue>
{
    private readonly FixationFacade<TFactValue> fixation;
    private readonly Dictionary<string, ModelInstance<TFactValue>> modelsByInstanceId
        = new();

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
        modelsByInstanceId.Add(modelInstance.InstanceId, modelInstance);
    }

    private void OnFactFixated(
        object sender,
        InstanceFactId fixatedFactId,
        bool isHappened)
    {
        var fact = fixation.ResolvedModelProvider.GetFact(fixatedFactId);
        resultBuilder.AddFact(fact);
    }

    public SimulationResult Run()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        fixation.Generator.FixateRootCauses();
        stopwatch.Stop();

        var res = resultBuilder.Build();
        res.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        return res;
    }
}
