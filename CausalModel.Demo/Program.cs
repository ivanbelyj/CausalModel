using CausalModel.Model;
using CausalModel.Factors;
using CausalModel.Fixation;
using CausalModel.Facts;
using CausalModel.Model.Serialization;
using CausalModel.Model.Blocks;
using CausalModel.Model.Providers;
using CausalModel.Demo;

// Todo:
// Если CauseId для WeightEdge не указан, выбор реализации абстрактного факта
// работает некорректно
// 


const string FACTS_FILE = "character-facts.json";

Console.WriteLine($"Enter file name or press Enter to use or create {FACTS_FILE} (example)");
string? fileName = Console.ReadLine();
if (string.IsNullOrEmpty(fileName))
    fileName = FACTS_FILE;

//const bool CREATE_FILE = true;
CausalModel<string> causalModel;
try
{
    if (/*!CREATE_FILE && */ File.Exists(fileName))
    {
        Console.WriteLine("Found " + fileName);
        causalModel = Deserialize(fileName)
            ?? throw new NullReferenceException("Deserialized model is null");
    }
    else if (fileName != FACTS_FILE && File.Exists(FACTS_FILE))
    {
        Console.WriteLine($"File {fileName} not found. Found {FACTS_FILE}, use example");
        fileName = FACTS_FILE;
        causalModel = Deserialize(fileName)
            ?? throw new NullReferenceException("Deserialized model is null");
    }
    else
    {
        Console.WriteLine($"File {fileName} not found. Create {FACTS_FILE}");

        causalModel = DemoBuilding.CreateDemoCausalModel();
        

        Serialize(causalModel, fileName);
        Console.WriteLine("Data used for generation saved to " + fileName);
    }
} catch (Exception ex)
{
    var prevColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("An error occured during the model opening.");
    Console.ForegroundColor = prevColor;

    Console.WriteLine(ex.ToString());
    Console.ReadKey(false);
    return;
}
ResolvingModelProvider<string> factsProvider = DemoBuilding
    .CreateModelProvider(causalModel, DemoBuilding.CreateDemoConventionMap());

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Press Enter to generate, Space to specify a seed"
        + " or something else to exit");
    var key = Console.ReadKey(false);
    if (key.Key == ConsoleKey.Enter)
    {
        Console.WriteLine();
        Generate(factsProvider);
    } else if (key.Key == ConsoleKey.Spacebar)
    {
        Console.WriteLine();
        Console.WriteLine("\nEnter seed (integer)");
        try
        {
            Generate(factsProvider, int.Parse(Console.ReadLine()!));
        } catch (FormatException)
        {
            Console.WriteLine("Integer expected");
        }
        
    } else
    {
        break;
    }
    Console.WriteLine("\n");
}

CausalModel<string>? Deserialize(string fileName)
{
    string fileContent = File.ReadAllText(fileName);
    var model = CausalModelSerialization.FromJson<string>(fileContent);
    return model;
}

string Serialize(CausalModel<string> model, string fileName = "fact-collection.json")
{
    string jsonString = CausalModelSerialization.ToJson<string>(model, true);
    if (!fileName.EndsWith(".json"))
    {
        fileName += ".json";
    }
    File.WriteAllText(fileName, jsonString);
    return jsonString;
}

void Generate(ResolvingModelProvider<string> factsProvider, int? seed = null)
{
    if (seed == null)
        seed = new Random().Next();

    Console.WriteLine("Seed: " + seed);
    Fixator<string> fixator = new Fixator<string>();
    var generator = new CausalGenerator<string>(factsProvider, fixator, seed.Value);
    fixator.FactFixated += OnFactHappened;

    generator.FixateRootCauses();
}

void OnFactHappened(object sender, Fact<string> fact, bool isHappened)
{
    if (isHappened)
    {
        Console.WriteLine((fact.IsRootCause() ? "" : "\t") + fact.FactValue);
    }
}

