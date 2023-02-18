using CausalModel;
using CausalModel.Factors;
using CausalModel.Nodes;
using System.Diagnostics;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Нажмите Enter, чтобы сгенерировать, или Space, чтобы указать seed");
    var key = Console.ReadKey(false);
    if (key.Key == ConsoleKey.Enter)
    {
        Console.WriteLine();
        Test1();
    } else if (key.Key == ConsoleKey.Spacebar)
    {
        Console.WriteLine();
        Console.WriteLine("\nВведите seed");
        Test1(int.Parse(Console.ReadLine()));
    } else
    {
        break;
    }
    Console.WriteLine("\n");
}

void Test1(int? seed = null)
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

    var factCollection = new FactCollection<string>(facts);

    if (seed == null)
        seed = new Random().Next();
    // Todo: seed 1345190346
    // 192771235
    Console.WriteLine("Seed: " + seed);
    var model = new CausalModel<string>(factCollection, seed.Value);
    model.FactHappened += OnFactHappened;

    model.FixateRoots();
    // model.Fixate(hobbyRoot.Id);
}

void OnFactHappened(Fact<string> fact)
{
    Console.WriteLine((fact.IsRootNode() ? "" : "\t") + fact.NodeValue);
}
