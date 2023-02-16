using CausalModel;
using CausalModel.Edges;
using CausalModel.Nodes;

Test1();

void Test1()
{
    // Следующая модель генерации персонажа используется исключительно для теста и демонстрации
    var facts = new List<Fact<string>>();
    Fact<string> hobbyRoot = FactUtils.CreateNode(0.9f, "Хобби");
    facts.Add(hobbyRoot);

    foreach (string hobbyName in new string[] { "Рисование",
        "Музыка", "Worldbuilding", "Программирование", "Gamedev",
        "Писательство", "Спорт", "Скульптура",
        "3d моделирование"})
    {
        facts.Add(FactUtils.CreateNode(0.5f, hobbyName, hobbyRoot.Id));
    }

    var educationNode = FactUtils.CreateNode(1, "Образование", null);
    facts.AddRange(FactUtils.CreateAbstractFact(educationNode,
        "Компьютерные науки", "История", "Математика"));
    var linguisticsNode = FactUtils.CreateVariant(educationNode.Id, 2, "лингвистика");
    facts.Add(linguisticsNode);

    var conlangHobby = FactUtils.CreateNode(1f, "Создание языков", hobbyRoot.Id);

    foreach (string nodeValue in new string[] { "Разбирается в лингвистике",
        "Понимает несколько языков" })
    {
        // Если персонаж - лингвист, вероятность повышается
        var linguisticsEdge = new ProbabilityEdge(1, linguisticsNode.Id);
        // Создание языков тоже повышает вероятность
        var conlangEdge = new ProbabilityEdge(0.5f, conlangHobby.Id);
        var node = FactUtils.CreateNodeWithOr(nodeValue, linguisticsEdge, conlangEdge);
        facts.Add(node);
    }

    // Раса напрямую связана с бытием существа.
    // Факт наличия расы реализуется одним из конкретных вариантов
    // CausalModelNode<string> raceNode = new CausalModelNode<string>(
    //    new ProbabilityNest(null, 1), "Раса");
    Fact<string> raceNode = FactUtils.CreateNode(1, "Раса", null);
    facts.AddRange(FactUtils.CreateAbstractFact(raceNode,
        "тшэайская", "мэрайская", "мйеурийская", "эвойская"));

    var factCollection = new FactCollection<string>(facts);
}
