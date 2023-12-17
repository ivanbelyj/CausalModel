using CausalModel.Fixation;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using CausalModel.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo;
public class CausalModelManager
{
    private const string FACTS_FILE = "character-facts.json";
    private readonly FileHandler fileHandler;

    private CausalModel<string>? causalModel;

    private IResolvedModelProvider<string>? modelProvider;

    public CausalModelManager()
    {
        fileHandler = new FileHandler();
    }

    public CausalModel<string> SetCausalModel(string fileName)
    {
        try
        {
            causalModel = DeserializeModel(fileName);
        }
        catch (Exception ex)
        {
            UserInteraction.PrintErrorMessage(
            "An error occured during the model opening.", ex);

            Console.ReadKey(false);

            throw;
        }

        return causalModel;
    }

    private CausalModel<string> DeserializeModel(string fileName)
    {
        CausalModel<string> causalModel;
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
        return causalModel;
    }

    public void RunModel()
    {
        if (causalModel == null)
            throw new InvalidOperationException("Causal model is not set to run");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to generate, Space to specify a seed"
                + " or something else to exit");
            var key = Console.ReadKey(false);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                Generate(causalModel);
            }
            else if (key.Key == ConsoleKey.Spacebar)
            {
                Console.WriteLine();
                Console.WriteLine("\nEnter seed (integer)");
                try
                {
                    Generate(causalModel, int.Parse(Console.ReadLine()!));
                }
                catch (FormatException)
                {
                    Console.WriteLine("Integer expected");
                }

            }
            else
            {
                break;
            }
            Console.WriteLine("\n");
        }
    }

    private void Generate(CausalModel<string> causalModel, int? seed = null)
    {
        seed ??= new Random().Next();

        Console.WriteLine("Seed: " + seed);

        Fixator<string> fixator = new();
        fixator.FactFixated += OnFactHappened;

        var generatorBuilder = new CausalGeneratorBuilder<string>(causalModel,
            seed.Value)
            .WithFixator(fixator)
            .WithConventions(DemoBuilding.CreateDemoConventionMap());

        var generator = generatorBuilder.Build();
        this.modelProvider = generatorBuilder.ResolvedModelProvider;

        generator.FixateRootCauses();
    }

    private void OnFactHappened(
        object sender,
        InstanceFactId fixatedFactId,
        bool isHappened)
    {
        if (isHappened)
        {
            var fact = modelProvider!.GetFact(fixatedFactId);

            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write((fact.Fact.IsRootCause() ? "" : "\t")
                + fact.Fact.FactValue);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" ({fixatedFactId})");

            Console.ForegroundColor = prevColor;
        }
    }
}
