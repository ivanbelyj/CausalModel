using CausalModel.CLI.Enums;
using CausalModel.Common;
using CausalModel.Fixation.Fixators;
using CausalModel.Fixation;
using CausalModel.Running.FixationRunning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Fixation.Fixators.Pending;
using CausalModel.Running.Models.FixationResult;

namespace CausalModel.CLI.Services;

internal class CausalFixationHelper
{
    private readonly CausalBundle<string> bundle;

    public CausalFixationHelper(CausalBundle<string> bundle)
    {
        this.bundle = bundle;
    }

    public FixationResult<string> Fixate(int? seed, FixatorType fixatorType)
    {
        var fixator = CreateFixator(fixatorType);
        var facade = CreateFacadeBuilder(fixator).Build();
        var fixationRunner = new FixationRunner<string>(facade);
        return fixationRunner.Run(seed);
    }

    private IFixator<string> CreateFixator(FixatorType fixatorType) =>
        fixatorType switch
        {
            FixatorType.Default => new Fixator<string>(),
            FixatorType.Pending => CreatePendingFixator(),
            _ => throw new ArgumentException($"Unsupported fixator type: {fixatorType}")
        };

    private PendingFixator<string> CreatePendingFixator()
    {
        var fixator = new PendingFixator<string>();
        //fixator.FactPending += (sender, instanceFact, isOccurred) => {
        //    var fact = instanceFact.Fact;
        //    WriteSubtle(
        //        $"Pending fact: {fact.FactValue} ({fact.Id}). " +
        //        $"is occurred: {isOccurred}");
        //};
        return fixator;
    }

    private FixationFacadeBuilder<string> CreateFacadeBuilder(
        IFixator<string>? fixator = null)
    {
        return new FixationFacadeBuilder<string>(bundle)
            .UseFixatorFactory(() => fixator ?? new Fixator<string>());
            //.AddOnFactFixated(HandleFactFixated)
            //.AddOnBlockImplemented((sender, block, convention, implementation) =>
            //    fixationResult.ImplementedBlocks.Add(block))
            //.AddOnModelInstanceCreated((sender, modelInstance) =>
            //    fixationResult.ModelInstantiated.Add(modelInstance));
    }
}
