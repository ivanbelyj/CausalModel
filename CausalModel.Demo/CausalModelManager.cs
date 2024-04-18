using CausalModel.Blocks.Resolving;
using CausalModel.Fixation;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using CausalModel.Model.Serialization;
using CausalModel.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo;
public class CausalModelManager
{
    private const string FACTS_FILE = "character-facts.json";
    private readonly FileHandler fileHandler;

    private CausalModel<string>? causalModel;
    private BlockResolvingMap<string>? blockResolvingMap;

    private CausalGenerator<string>? generator;

    public CausalModelManager()
    {
        fileHandler = new FileHandler();
    }

    public void Init(string fileName)
    {
        try
        {
            SetModelAndResolvingMap(fileName);
        }
        catch (Exception ex)
        {
            UserInteraction.PrintErrorMessage(
                "An error occured during the model opening.", ex);

            Console.ReadKey(false);

            throw;
        }
    }

    private void SetModelAndResolvingMap(string fileName)
    {
        if (/*!CREATE_FILE && */ File.Exists(fileName))
        {
            Console.WriteLine("Found " + fileName);
            causalModel = fileHandler.Deserialize(fileName)
                ?? throw new NullReferenceException("Deserialized model is null");
        }
        else if (fileName != FACTS_FILE && File.Exists(FACTS_FILE))
        {
            Console.WriteLine($"File {fileName} not found. Found {FACTS_FILE}, use example");
            fileName = FACTS_FILE;
            causalModel = fileHandler.Deserialize(fileName)
                ?? throw new NullReferenceException("Deserialized model is null");
        }
        else
        {
            Console.WriteLine($"File {fileName} not found. Create {FACTS_FILE}");

            causalModel = DemoBuilding.CreateDemoCausalModel();

            fileHandler.Serialize(causalModel, fileName);
            Console.WriteLine("Data used for generation saved to " + fileName);
        }

        // Todo: get it from file ?
        blockResolvingMap = DemoBuilding.CreateDemoConventionMap();
    }

    public void RunModel()
    {
        if (causalModel == null)
            throw new InvalidOperationException("Causal model is not set to run");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to generate, Space to specify a seed,"
                + " Tab to run Monte-Carlo simulation "
                + " or something else to exit");
            var key = Console.ReadKey(false);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                Generate();
            }
            else if (key.Key == ConsoleKey.Spacebar)
            {
                Console.WriteLine();
                Console.WriteLine("\nEnter seed (integer)");
                try
                {
                    Generate(int.Parse(Console.ReadLine()!));
                }
                catch (FormatException)
                {
                    Console.WriteLine("Integer expected");
                }

            } else if (key.Key == ConsoleKey.Tab)
            {
                RunMonteCarloSimulation();
            }
            else
            {
                break;
            }
            Console.WriteLine("\n");
        }
    }

    private void Generate(int? seed = null)
    {
        if (causalModel == null || blockResolvingMap == null)
            throw new InvalidOperationException("Causal model or resolving map "
                + "is null. Cannot generate");

        seed ??= new Random().Next();

        Console.WriteLine("Seed: " + seed);

        var facadeBuilder = new FixationFacadeBuilder<string>(causalModel)
            .AddOnFactFixated(OnFactFixated)
            .WithResolvingMap(blockResolvingMap)
            .AddOnBlockImplemented((sender, block, convention, implementation) =>
            {
                Console.WriteLine($"// Block implemented: {block.Id}");
            })
            .AddOnModelInstanceCreated((sender, ModelInstance) =>
            {
                Console.WriteLine($"// Model instantiated: {ModelInstance.Model.Name}"
                    + $"{ModelInstance.InstanceId}");
            });

        var fixationFacade = facadeBuilder.Build();
        generator = fixationFacade.CreateGenerator();

        generator.FixateRootFacts();
    }

    private void RunMonteCarloSimulation()
    {
        if (causalModel == null || blockResolvingMap == null)
            throw new InvalidOperationException("Causal model or resolving map "
                + "is null. Cannot run Monte-Carlo simulation");

        var facadeBuilder = new FixationFacadeBuilder<string>(causalModel)
            .WithResolvingMap(blockResolvingMap);
        SimulationsRunner<string> simulationsRunner = new(facadeBuilder);

        Console.WriteLine("\nRunning Monte-Carlo simulation...");
        var totalResult = simulationsRunner.RunSimulations(1000);

        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n\nSimulation completed.\n");
        Console.ForegroundColor = prevColor;

        Console.WriteLine(totalResult.ToString(causalModel, blockResolvingMap));
    }

    private void OnFactFixated(
        object sender,
        InstanceFact<string> fixatedFact,
        bool isOccurred)
    {
        if (isOccurred)
        {
            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write((fixatedFact.Fact.IsRootCause() ? "" : "\t")
                + fixatedFact.Fact.FactValue);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" ({fixatedFact.InstanceFactId})");

            Console.ForegroundColor = prevColor;
        }
    }
}
