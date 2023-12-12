using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Factors;
using CausalModel.Facts;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo;
public class DemoBuilding
{
    public static ResolvingModelProvider<string> CreateDemoModelProvider()
    {
        return CreateModelProvider(CreateDemoCausalModel(), CreateDemoConventionMap());
    }

    public static ResolvingModelProvider<string> CreateModelProvider(
         CausalModel<string> model, 
         BlockResolvingMap<string> conventions)
    {
        var resolver = new BlockResolver<string>(conventions);
        resolver.BlockImplemented += (sender, block, convention, implementation) =>
        {
            Console.WriteLine($"// Block implemented: {block.Id}");
        };

        // Todo: should create instance here?
        return new ResolvingModelProvider<string>(new ModelInstance<string>(model),
            resolver);
    }

    public static BlockResolvingMap<string> CreateDemoConventionMap()
    {
        return new BlockResolvingMap<string>()
        {
            ModelsByConventionName = new Dictionary<string, CausalModel<string>>()
            {
                { "TestConvention", CreateConventionAndImplementation().model }
            }
        };
    }

    public static CausalModel<string> CreateDemoCausalModel()
    {
        var facts = CreateCharacterFacts();

        // Required for the declared block
        facts.Add(FactsBuilding.CreateFact(0.9f, "Block cause", null, "BlockCause"));

        var causalModel = new CausalModel<string>()
        {
            Facts = facts,
            BlockConventions = new List<BlockConvention>()
            {
                CreateConventionAndImplementation().convention
            },
            Blocks = new List<DeclaredBlock>()
            {
                new DeclaredBlock("block1", "TestConvention")
            }
        };
        return causalModel;
    }

    private static (BlockConvention convention, CausalModel<string> model)
        CreateConventionAndImplementation()
    {
        var conv = new BlockConvention("TestConvention")
        {
            Causes = new()
                {
                    new Factor()
                    {
                        // Cause from the parent model required for the block
                        CauseId = "BlockCause"
                    }
                },
            Consequences = new()
                {
                    new BaseFact()
                    {
                        Id = "BlockConsequence"
                    }
                }
        };

        var fact1 = FactsBuilding.CreateFact(1, "Inner fact 1", null);
        var fact2 = FactsBuilding.CreateFact(1, "Inner fact 2", fact1.Id);
        var impl = new CausalModel<string>()
        {
            
            Facts = new()
            {
                fact1,
                fact2,
                FactsBuilding.CreateFact(1, "Inner fact 3", null),
                FactsBuilding.CreateFact(1, "Inner fact 4", fact2.Id,
                    "BlockConsequence"),
            }
        };
        return (conv, impl);
    }

    private static List<Fact<string>> CreateCharacterFacts()
    {
        // A simple example of a character model

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

        return facts;
    }

}
