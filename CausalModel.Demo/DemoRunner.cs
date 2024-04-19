using CausalModel.Blocks.Resolving;
using CausalModel.Demo.Utils;
using CausalModel.Fixation;
using CausalModel.Fixation.Fixators;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using CausalModel.Model.Serialization;
using CausalModel.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CausalModel.Demo.Utils.WriteUtils;

namespace CausalModel.Demo;
public class DemoRunner
{
    private readonly SourceData sourceData;

    private CausalModel<string> CausalModel => sourceData.CausalModel;
    private BlockResolvingMap<string> BlockResolvingMap
        => sourceData.BlockResolvingMap;

    public DemoRunner(SourceData sourceData)
    {
        this.sourceData = sourceData;
    }

    public DemoRunner(string sourceDataFileName)
    {
        sourceData = DemoUtils.GetSourceData(sourceDataFileName);
    }

    public void Run()
    {
        bool shouldContinue = true;
        while (shouldContinue)
        {
            Line();
            WriteMain("Press a key");
            Write("\t"
                + "Enter - generate\n\t"
                + "\\ - generate using pending fixator\n\t"
                + "Space - specify a seed\n\t"
                + "Tab - run Monte-Carlo simulation\n\t"
                //+ "` - run Monte-Carlo simulation using pending fixator\n\t"
                + "Else - exit");
            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    Line();
                    Generate();
                    break;
                case ConsoleKey.Oem5:  // \
                    Line();
                    GeneratePending();
                    break;
                case ConsoleKey.Spacebar:
                    Line();
                    Write("\nEnter seed (integer)");
                    try
                    {
                        Generate(seed: int.Parse(Console.ReadLine()!));
                    }
                    catch (FormatException)
                    {
                        Write("Integer expected");
                    }
                    break;
                case ConsoleKey.Tab:
                    RunMonteCarloSimulation();
                    break;
                //case ConsoleKey.Oem3:  // `
                //    Write("Now not implemented");
                //    break;
                default:
                    shouldContinue = false;
                    break;
            }

            Write("\n");
        }
    }

    private void Generate(int? seed = null)
    {
        MeasureExecutionTime(() =>
        {
            var generator = CreateGenerator(new Fixator<string>(), seed);
            generator.FixateRootFacts();
        });
    }

    private void GeneratePending(int? seed = null)
    {
        MeasureExecutionTime(() =>
        {
            var pendingFixator = CreatePendingFixator();
            var generator = CreateGenerator(pendingFixator, seed);
            generator.FixateRootFacts();

            while (pendingFixator.HasPendingFacts())
            {
                Write($"\nApprove call " +
                    $"{pendingFixator.ApprovePendingFactsCallsCount + 1}");
                pendingFixator.ApprovePendingFacts();
            }
        });
    }

    private void MeasureExecutionTime(Action action)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        Write("\n---------------------");
        Write($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
    }

    private CausalGenerator<string> CreateGenerator(
        IFixator<string> fixator,
        int? seed = null)
    {
        seed ??= new Random().Next();

        Write("Seed: " + seed);
        
        var facadeBuilder = CreateFacadeBuilder(fixator);
        var fixationFacade = facadeBuilder.Build();

        return fixationFacade.CreateGenerator();
    }

    private PendingFixator<string> CreatePendingFixator()
    {
        var fixator = new PendingFixator<string>();
        fixator.FactPending += (sender, instanceFact, isOccurred) => {
            var fact = instanceFact.Fact;
            WriteSubtle($"Pending fact: {fact.FactValue} ({fact.Id}). " +
                $"is occurred: {isOccurred}");
        };
        return fixator;
    }

    private FixationFacadeBuilder<string> CreateFacadeBuilder(
        IFixator<string>? fixator = null)
    {
        return new FixationFacadeBuilder<string>(CausalModel)
            .WithFixator(fixator ?? new Fixator<string>())
            .AddOnFactFixated(DemoUtils.WriteFactFixated)
            .WithResolvingMap(BlockResolvingMap)
            .AddOnBlockImplemented((sender, block, convention, implementation) =>
            {
                Write($"// Block implemented: {block.Id}");
            })
            .AddOnModelInstanceCreated((sender, ModelInstance) =>
            {
                Write($"// Model instantiated: {ModelInstance.Model.Name}"
                    + $"{ModelInstance.InstanceId}");
            });
    }

    private void RunMonteCarloSimulation(bool? usePendingFixator = null)
    {
        var facadeBuilder = new FixationFacadeBuilder<string>(CausalModel)
            .WithResolvingMap(BlockResolvingMap);
        SimulationsRunner<string> simulationsRunner = new(facadeBuilder);

        Write("\nRunning Monte-Carlo simulation...");
        var totalResult = simulationsRunner.RunSimulations(1000);

        WriteMain("\n\nSimulation completed.\n");

        Write(totalResult.ToString(CausalModel, BlockResolvingMap));
    }
}
