using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Factors;
using CausalModel.Facts;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo.Utils;
public static class BuildingUtils
{
    private const string CausalModelName = "Character Model";

    public static BlockResolvingMap<string> CreateDemoConventionMap()
    {
        return new BlockResolvingMap<string>()
        {
            ModelsByConventionName = new Dictionary<string, CausalModel<string>>()
            {
                { "TestConvention", CreateDemoConventionImplementation() }
            }
        };
    }

    private static BlockCausesConvention CreateDemoCausesConvention()
    {
        var conv = new BlockCausesConvention("TestCausesConvention")
        {
            // Cause from the parent model required for the block
            Causes = new() { "BlockCause" }
        };

        return conv;
    }

    private static BlockConvention CreateDemoConvention()
    {
        var conv = new BlockConvention("TestConvention")
        {
            Consequences = new() { "BlockConsequence" }
        };

        return conv;
    }

    private static CausalModel<string> CreateDemoConventionImplementation()
    {
        var fact1 = FactBuilding.CreateFact(1, "Inner fact 1", null);
        var fact2 = FactBuilding.CreateFact(1, "Inner fact 2", fact1.Id);
        var impl = new CausalModel<string>
        {
            Facts = new()
            {
                fact1,
                fact2,
                FactBuilding.CreateFact(1, "Inner fact 3", null),
                FactBuilding.CreateFact(
                    probability: 1,
                    value: "Block consequence (can be used in the parent model)",
                    causeId: fact2.Id,
                    id: "BlockConsequence"),
                FactBuilding.CreateFact(
                    probability: 1,
                    value: "! Fact using external cause",
                    causeId: "BlockCause"),
            },
            Name = "Test model (used as block)"
        };
        return impl;
    }

    public static CausalModel<string> CreateDemoCausalModel()
    {
        var facts = CreateCharacterFacts();

        // Required for the declared block
        facts.Add(FactBuilding.CreateFact(
            0.9f,
            value: "Block cause fact value",
            causeId: null,
            id: "BlockCause1"));

        facts.Add(FactBuilding.CreateFact(
            0.9f,
            value: "Block cause fact value (2)",
            causeId: null,
            id: "BlockCause2"));

        // Add fact using block consequence
        facts.Add(FactBuilding.CreateFact(1f,
            value: "Fact using block consequence",
            causeId: "BlockConsequence"));

        var causalModel = new CausalModel<string>
        {
            Facts = facts,
            BlockConventions = new List<BlockConvention>()
            {
                CreateDemoConvention()
            },
            BlockCausesConventions = new List<BlockCausesConvention>()
            {
                CreateDemoCausesConvention()
            },
            DeclaredBlocks = new List<DeclaredBlock>()
            {
                CreateDeclaredBlock(1),
                CreateDeclaredBlock(2)
            },
            Name = CausalModelName
        };
        return causalModel;
    }

    private static DeclaredBlock CreateDeclaredBlock(int blockNumber)
    {
        return new DeclaredBlock(
            $"Block{blockNumber}",
            "TestConvention",
            "TestCausesConvention",
            new()
            {
                { "BlockCause", $"BlockCause{blockNumber}" }
            });
    }

    private static List<Fact<string>> CreateCharacterFacts()
    {
        // A simple example of a character model

        var facts = new List<Fact<string>>();
        Fact<string> hobbyRoot = FactBuilding.CreateFact(0.9f, "Хобби");
        facts.Add(hobbyRoot);

        foreach (string hobbyName in new string[] { "Рисование",
            "Гитара", "Программирование", "Gamedev",
            "Писательство", "Спорт", "Role play",
            "3d моделирование"})
        {
            facts.Add(FactBuilding.CreateFact(0.3f, hobbyName, hobbyRoot.Id));
        }
        foreach (string hobbyName in new string[] { "Worldbuilding" })
        {
            facts.Add(FactBuilding.CreateFact(0.1f, hobbyName, hobbyRoot.Id));
        }

        var educationNode = FactBuilding.CreateFact(1, "Образование", null);
        facts.Add(educationNode);
        facts.AddRange(FactBuilding.CreateAbstractFactVariants(educationNode,
            "Компьютерные науки", "История", "Математика"));
        var linguisticsNode = FactBuilding.CreateVariant(educationNode.Id,
            20, "лингвистика");
        facts.Add(linguisticsNode);

        var conlangHobby = FactBuilding.CreateFact(0.1f, "Создание языков", hobbyRoot.Id);
        facts.Add(conlangHobby);

        // Причиной того, что персонаж понимает несколько языков, может быть как
        // создание языков, так и образование.
        // Других причин в данной модели не предполагается
        var linguisticsEdge = new ProbabilityFactor(0.9f, linguisticsNode.Id);
        var conlangEdge = new ProbabilityFactor(0.3f, conlangHobby.Id);
        var languages = FactBuilding.CreateFactWithOr("Понимает несколько языков",
            linguisticsEdge, conlangEdge);
        // Todo: если оба ребра имеют ненулевую вероятность, то все зависит от conlangEdge
        // (первое не учитывается). Если есть только одно ребро - учитывается только оно
        facts.Add(languages);

        // Персонаж с лингвистическим образованием будет разбираться в лингвистике
        // почти гарантированно, а тот, кто создает языки - 20%
        var linguisticsPro = FactBuilding.CreateFactWithOr("Разбирается в лингвистике",
            new ProbabilityFactor(0.95f, linguisticsNode.Id),
            new ProbabilityFactor(0.2f, conlangHobby.Id)
            );
        facts.Add(linguisticsPro);

        // Раса напрямую связана с бытием существа.
        // Факт наличия расы реализуется одним из конкретных вариантов
        Fact<string> raceNode = FactBuilding.CreateFact(1, "Раса", null);
        facts.Add(raceNode);
        facts.AddRange(FactBuilding.CreateAbstractFactVariants(raceNode,
            "тшэайская", "мэрайская", "мйеурийская", "эвойская", "оанэйская"));

        return facts;
    }
}
