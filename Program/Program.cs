using CausalModel;
using CausalModel.FactCollection;
using CausalModel.Factors;
using CausalModel.Model;
using CausalModel.Nodes;
using System.Diagnostics;

const string FACTS_FILE = "character-facts.json";
FactCollection<string> factCol;
if (File.Exists(FACTS_FILE))
{
    Console.WriteLine("Найден файл " + FACTS_FILE);
    factCol = Deserialize(FACTS_FILE);
} else
{
    Console.WriteLine("Коллекция фактов была создана программно");
    factCol = CreateCharacterFactsCollection();
}

Serialize(factCol, FACTS_FILE);
Console.WriteLine("Данные для генерации сохранены в файл " + FACTS_FILE);

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Нажмите Enter, чтобы сгенерировать, или Space, чтобы указать seed");
    var key = Console.ReadKey(false);
    if (key.Key == ConsoleKey.Enter)
    {
        Console.WriteLine();
        Generate(factCol);
    } else if (key.Key == ConsoleKey.Spacebar)
    {
        Console.WriteLine();
        Console.WriteLine("\nВведите seed (целое число)");
        try
        {
            Generate(factCol, int.Parse(Console.ReadLine()));
        } catch (FormatException)
        {
            Console.WriteLine("Ожидалось целое число");
        }
        
    } else
    {
        break;
    }
    Console.WriteLine("\n");
}

FactCollection<string>? Deserialize(string fileName)
{
    string fileContent = File.ReadAllText(fileName);
    var serializer = new FactCollectionSerializer();
    var factCol = serializer.FromJson<string>(fileContent);
    return factCol;
}

string Serialize(FactCollection<string> factCollection, string fileName = "fact-collection.json")
{
    var serializer = new FactCollectionSerializer();
    string jsonString = serializer.ToJson(factCollection, true);
    if (!fileName.EndsWith(".json"))
    {
        fileName += ".json";
    }
    File.WriteAllText(fileName, jsonString);
    return jsonString;
}

void Generate(FactCollection<string> factCollection, int? seed = null)
{
    if (seed == null)
        seed = new Random().Next();

    // Todo: seed 1345190346
    // 192771235
    Console.WriteLine("Seed: " + seed);
    var model = new CausalModel<string>(factCollection, seed.Value);
    model.FactHappened += OnFactHappened;

    model.FixateRoots();
}

void OnFactHappened(Fact<string> fact)
{
    Console.WriteLine((fact.IsRootNode() ? "" : "\t") + fact.NodeValue);
}

FactCollection<string> CreateCharacterFactsCollection()
{
    // Пример простейшей каузальной модели персонажа
    var facts = new List<Fact<string>>();
    Fact<string> hobbyRoot = FactUtils.CreateNode(0.9f, "Хобби");
    facts.Add(hobbyRoot);

    foreach (string hobbyName in new string[] { "Рисование",
        "Гитара", "Программирование", "Gamedev",
        "Писательство", "Спорт", "Role play",
        "3d моделирование"})
    {
        facts.Add(FactUtils.CreateNode(0.3f, hobbyName, hobbyRoot.Id));
    }
    foreach (string hobbyName in new string[] { "Worldbuilding", "" })
    {
        facts.Add(FactUtils.CreateNode(0.1f, hobbyName, hobbyRoot.Id));
    }

    var educationNode = FactUtils.CreateNode(1, "Образование", null);
    facts.AddRange(FactUtils.CreateAbstractFact(educationNode,
        "Компьютерные науки", "История", "Математика"));
    var linguisticsNode = FactUtils.CreateVariant(educationNode.Id, 20, "лингвистика");
    facts.Add(linguisticsNode);

    var conlangHobby = FactUtils.CreateNode(0.1f, "Создание языков", hobbyRoot.Id);
    facts.Add(conlangHobby);

    // Причиной того, что персонаж понимает несколько языков, может быть как
    // создание языков, так и образование.
    // Других причин в данной модели не предполагается
    var linguisticsEdge = new ProbabilityFactor(0.9f, linguisticsNode.Id);
    var conlangEdge = new ProbabilityFactor(0.3f, conlangHobby.Id);
    var languages = FactUtils.CreateNodeWithOr("Понимает несколько языков",
        linguisticsEdge, conlangEdge);
    // Todo: если оба ребра имеют ненулевую вероятность, то все зависит от conlangEdge
    // (первое не учитывается). Если есть только одно ребро - учитывается только оно
    facts.Add(languages);

    // Персонаж с лингвистическим образованием будет разбираться в лингвистике
    // почти гарантированно, а тот, кто создает языки - 20%
    var linguisticsPro = FactUtils.CreateNodeWithOr("Разбирается в лингвистике",
        new ProbabilityFactor(0.95f, linguisticsNode.Id),
        new ProbabilityFactor(0.2f, conlangHobby.Id)
        );
    facts.Add(linguisticsPro);

    // Раса напрямую связана с бытием существа.
    // Факт наличия расы реализуется одним из конкретных вариантов
    Fact<string> raceNode = FactUtils.CreateNode(1, "Раса", null);
    facts.AddRange(FactUtils.CreateAbstractFact(raceNode,
        "тшэайская", "мэрайская", "мйеурийская", "эвойская", "оанэйская"));

    return new FactCollection<string>(facts);
}
