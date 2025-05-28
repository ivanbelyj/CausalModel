using CausalModel.Common;
using CausalModel.Common.DataProviders;
using CausalModel.Demo.Utils;
using CausalModel.Fixation;
using CausalModel.Fixation.Fixators;
using CausalModel.Fixation.Fixators.Pending;
using CausalModel.Running.ProbabilityEstimation;
using System;
using System.Diagnostics;

using static CausalModel.Demo.Utils.WriteUtils;

namespace CausalModel.Demo;
public class DemoRunner
{
    private readonly CausalBundle<string> bundle;

    public DemoRunner(CausalBundle<string> sourceData)
    {
        this.bundle = sourceData;
    }

    public DemoRunner(string sourceDataFileName)
    {
        bundle = DemoUtils.GetSourceData(sourceDataFileName);
    }

    public void Run()
    {
        try
        {
            RunLoop();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred. Press any key to exit. {ex}");
            Console.ReadKey(false);
        }
    }

    private void RunLoop()
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
        var generator = CreateGenerator(new Fixator<string>(), seed);
        MeasureExecutionTime(() =>
        {
            generator.FixateRootFacts();
        });
    }

    private void GeneratePending(int? seed = null)
    {
        var pendingFixator = CreatePendingFixator();
        var generator = CreateGenerator(pendingFixator, seed);
        MeasureExecutionTime(() =>
        {
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
        Write($"Execution time: {stopwatch.Elapsed.TotalNanoseconds:N0} ns ({stopwatch.ElapsedMilliseconds} ms)");
    }

    private CausalGenerator<string> CreateGenerator(
        IFixator<string> fixator,
        int? seed = null)
    {
        seed ??= new Random().Next();

        Write("Seed: " + seed);
        
        var facadeBuilder = CreateFacadeBuilder(fixator);
        var fixationFacade = facadeBuilder.Build();

        return fixationFacade.CreateGenerator(seed);
    }

    private PendingFixator<string> CreatePendingFixator()
    {
        var fixator = new PendingFixator<string>();
        fixator.FactPending += (sender, instanceFact, isOccurred) => {
            var fact = instanceFact.Fact;
            WriteSubtle(
                $"Pending fact: {fact.FactValue} ({fact.Id}). " +
                $"is occurred: {isOccurred}");
        };
        return fixator;
    }

    private FixationFacadeBuilder<string> CreateFacadeBuilder(
        IFixator<string>? fixator = null)
    {
        return new FixationFacadeBuilder<string>(bundle)
            .UseFixatorFactory(() => fixator ?? new Fixator<string>())
            .AddOnFactFixated(DemoUtils.WriteFactFixated)
            .AddOnBlockImplemented((sender, block, convention, implementation) =>
            {
                Write($"// Block implemented: {block.Id}");
            })
            .AddOnModelInstanceCreated((sender, modelInstance) =>
            {
                Write(
                    $"// Model instantiated: {modelInstance.Model.Name}" +
                    $"{modelInstance.InstanceId}");
            });
    }

    private void RunMonteCarloSimulation(bool? usePendingFixator = null)
    {
        var facadeBuilder = new FixationFacadeBuilder<string>(bundle);
        var probabilityEstimator = new ProbabilityEstimator<string>(facadeBuilder);

        Write("\nRunning Monte-Carlo simulation...");
        var totalResult = probabilityEstimator.RunSimulations(10000);

        WriteMain("\n\nSimulation completed.\n");

        Write(totalResult.ToString());
    }
}
