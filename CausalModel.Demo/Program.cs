using CausalModel.Model;
using CausalModel.Factors;
using CausalModel.Fixation;
using CausalModel.Facts;
using CausalModel.Model.Serialization;
using CausalModel.Blocks;
using CausalModel.Blocks.BlockReferences;

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
    //if (/*!CREATE_FILE && */ File.Exists(fileName))
    //{
    //    Console.WriteLine("Found " + fileName);
    //    causalModel = Deserialize(fileName);
    //}
    //else if (fileName != FACTS_FILE && File.Exists(FACTS_FILE))
    //{
    //    Console.WriteLine($"File {fileName} not found. Found {FACTS_FILE}, use example");
    //    fileName = FACTS_FILE;
    //    causalModel = Deserialize(fileName);
    //}
    //else
    {
        Console.WriteLine($"File {fileName} not found. Create {FACTS_FILE}");
        var facts = CreateCharacterFactsCollection();

        var referencesList = new[] {
                "Cause1", "Cause2"
            }
            .Select(x => new AbstractReference() {
                Name = x
            })
            .OfType<BlockReference>()
            .ToList();

        referencesList
            .Add(new SpecifiedReference()
            {
                Id = Guid.NewGuid()
            });

        causalModel = new CausalModel<string>()
        {
            Facts = facts,
            BlockConventions = new List<BlockConvention>()
            {
                new BlockConvention()
                {
                    Name = "TestConvention",
                    //Causes = new BlockReference[] { }
                    Causes = referencesList,
                },
            },
            Blocks = new List<DeclaredBlock>()
            {
                new DeclaredBlock()
                {
                    ConventionName = "TestConvention",
                    Name = "block1"
                }
            }
        };
        Serialize(causalModel, fileName);
        Console.WriteLine("Data used for generation saved to " + fileName);

    }
} catch (Exception ex)
{
    var prevColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Exception");
    Console.ForegroundColor = prevColor;

    Console.WriteLine(ex.ToString());
    Console.ReadKey(false);
    return;
}


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
    } else if (key.Key == ConsoleKey.Spacebar)
    {
        Console.WriteLine();
        Console.WriteLine("\nEnter seed (integer)");
        try
        {
            Generate(causalModel, int.Parse(Console.ReadLine()));
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

void Generate(CausalModel<string> model, int? seed = null)
{
    if (seed == null)
        seed = new Random().Next();

    Console.WriteLine("Seed: " + seed);
    Fixator<string> fixator = new Fixator<string>();
    var generator = new CausalGenerator<string>(model, seed.Value, fixator);
    fixator.FactFixated += OnFactHappened;

    generator.FixateRoots();
}

void OnFactHappened(object sender, Fact<string> fact, bool isHappened)
{
    if (isHappened)
    {
        Console.WriteLine((fact.IsRootCause() ? "" : "\t") + fact.NodeValue);
    }
}

FactCollection<string> CreateCharacterFactsCollection()
{
    // Simple character model example
    var facts = new List<Fact<string>>();
    Fact<string> hobbyRoot = FactsBuilding.CreateFact(0.9f, "Хобби");
    facts.Add(hobbyRoot);

    foreach (string hobbyName in new string[] { "Рисование",
        "Гитара", "Программирование", "Gamedev",
        "Писательство", "Спорт", "Role play",
        "3d моделирование"})
    {
        facts.Add(FactsBuilding.CreateFact(0.3f, hobbyName, hobbyRoot.Id));
    }
    foreach (string hobbyName in new string[] { "Worldbuilding" })
    {
        facts.Add(FactsBuilding.CreateFact(0.1f, hobbyName, hobbyRoot.Id));
    }

    var educationNode = FactsBuilding.CreateFact(1, "Образование", null);
    facts.AddRange(FactsBuilding.CreateAbstractFact(educationNode,
        "Компьютерные науки", "История", "Математика"));
    var linguisticsNode = FactsBuilding.CreateVariant(educationNode.Id,
        20, "лингвистика");
    facts.Add(linguisticsNode);

    var conlangHobby = FactsBuilding.CreateFact(0.1f, "Создание языков", hobbyRoot.Id);
    facts.Add(conlangHobby);

    // Причиной того, что персонаж понимает несколько языков, может быть как
    // создание языков, так и образование.
    // Других причин в данной модели не предполагается
    var linguisticsEdge = new ProbabilityFactor(0.9f, linguisticsNode.Id);
    var conlangEdge = new ProbabilityFactor(0.3f, conlangHobby.Id);
    var languages = FactsBuilding.CreateFactWithOr("Понимает несколько языков",
        linguisticsEdge, conlangEdge);
    // Todo: если оба ребра имеют ненулевую вероятность, то все зависит от conlangEdge
    // (первое не учитывается). Если есть только одно ребро - учитывается только оно
    facts.Add(languages);

    // Персонаж с лингвистическим образованием будет разбираться в лингвистике
    // почти гарантированно, а тот, кто создает языки - 20%
    var linguisticsPro = FactsBuilding.CreateFactWithOr("Разбирается в лингвистике",
        new ProbabilityFactor(0.95f, linguisticsNode.Id),
        new ProbabilityFactor(0.2f, conlangHobby.Id)
        );
    facts.Add(linguisticsPro);

    // Раса напрямую связана с бытием существа.
    // Факт наличия расы реализуется одним из конкретных вариантов
    Fact<string> raceNode = FactsBuilding.CreateFact(1, "Раса", null);
    facts.AddRange(FactsBuilding.CreateAbstractFact(raceNode,
        "тшэайская", "мэрайская", "мйеурийская", "эвойская", "оанэйская"));

    return new FactCollection<string>(facts);
}
