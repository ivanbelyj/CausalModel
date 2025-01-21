using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Common;
using CausalModel.Factors;
using CausalModel.Facts;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo.Utils;
public static class DemoBundleBuildingUtils
{
    private const string CharacterModelName = "Character Model";
    private const string BlockModelName = "Test model (used as block)";

    public static CausalBundle<string> CreateDemoCausalBundle()
    {
        return new CausalBundle<string>()
        {
            CausalModels = new List<CausalModel<string>>
            {
                CreateDemoCausalModel(),
                CreateDemoConventionImplementation()
            },
            BlockConventions = new List<BlockConvention>()
            {
                CreateDemoConvention()
            },
            BlockCausesConventions = new List<BlockCausesConvention>()
            {
                CreateDemoCausesConvention()
            },
            BlockResolvingMap = CreateDemoBlockResolvingMap(),
            DefaultMainModel = CharacterModelName
        };
    }

    private static BlockResolvingMap CreateDemoBlockResolvingMap()
    {
        return new BlockResolvingMap()
        {
            ModelNamesByConventionName = new Dictionary<string, string>()
            {
                { "TestConvention", BlockModelName }
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
        var impl = new CausalModel<string>(BlockModelName)
        {
            Facts = new()
            {
                fact1,
                fact2,
                FactBuilding.CreateFact(1, "Inner fact 3", null),
                FactBuilding.CreateFact(
                    probability: 1,
                    value: "! Fact using external cause",
                    causeId: "BlockCause",
                    id: "fact1"),
                FactBuilding.CreateFact(
                    probability: 1,
                    value: "Block consequence (can be used in the parent model)",
                    causeId: "fact1",
                    id: "BlockConsequence"),
            },
        };
        return impl;
    }

    private static CausalModel<string> CreateDemoCausalModel()
    {
        var facts = CreateCharacterFacts();
        AddFactsRelatedToDeclaredBlocks(facts);
        
        var causalModel = new CausalModel<string>(CharacterModelName)
        {
            Facts = facts,
            DeclaredBlocks = new List<DeclaredBlock>()
            {
                CreateDeclaredBlock(1),
                CreateDeclaredBlock(2)
            }
        };
        return causalModel;
    }

    private static void AddFactsRelatedToDeclaredBlocks(List<Fact<string>> facts)
    {
        // Required for the declared block
        facts.Add(FactBuilding.CreateFact(
            0.7f,
            value: "Block cause fact value (1)",
            causeId: null,
            id: "BlockCause1"));

        facts.Add(FactBuilding.CreateFact(
            0.7f,
            value: "Block cause fact value (2)",
            causeId: null,
            id: "BlockCause2"));

        // Add facts using blocks consequences
        facts.Add(FactBuilding.CreateFact(1f,
            value: "Fact using block consequence (1)",
            causeId: "BlockConsequence1"));

        facts.Add(FactBuilding.CreateFact(1f,
            value: "Fact using block consequence (2)",
            causeId: "BlockConsequence2"));
    }

    private static DeclaredBlock CreateDeclaredBlock(int blockNumber)
    {
        return new DeclaredBlock(
            id: $"Block{blockNumber}",
            convention: "TestConvention",
            causesConvention: "TestCausesConvention",
            blockCausesMap: new()
            {
                { "BlockCause", $"BlockCause{blockNumber}" }
            },
            blockConsequencesMap: new()
            {
                { "BlockConsequence", $"BlockConsequence{blockNumber}" }
            });
    }

    private static List<Fact<string>> CreateCharacterFacts()
    {
        // A simple example of a character model

        var facts = new List<Fact<string>>();
        Fact<string> hobbyRoot = FactBuilding.CreateFact(0.9f, "�����");
        facts.Add(hobbyRoot);

        foreach (string hobbyName in new string[] {
            "���������", "������", "����������������", "Gamedev",
            "������������", "�����", "Role play", "3d �������������"})
        {
            facts.Add(FactBuilding.CreateFact(0.3f, hobbyName, hobbyRoot.Id));
        }
        foreach (string hobbyName in new string[] { "Worldbuilding" })
        {
            facts.Add(FactBuilding.CreateFact(0.1f, hobbyName, hobbyRoot.Id));
        }

        var educationNode = FactBuilding.CreateFact(1, "�����������", null);
        facts.Add(educationNode);
        facts.AddRange(FactBuilding.CreateAbstractFactVariants(educationNode,
            "������������ �����", "�������", "����������"));
        var linguisticsNode = FactBuilding.CreateVariant(educationNode.Id,
            20, "�����������");
        facts.Add(linguisticsNode);

        var conlangHobby = FactBuilding.CreateFact(0.1f, "�������� ������", hobbyRoot.Id);
        facts.Add(conlangHobby);

        // �������� ����, ��� �������� �������� ��������� ������, ����� ���� ���
        // �������� ������, ��� � �����������.
        // ������ ������ � ������ ������ �� ��������������
        var linguisticsEdge = new ProbabilityFactor(0.9f, linguisticsNode.Id);
        var conlangEdge = new ProbabilityFactor(0.3f, conlangHobby.Id);
        var languages = FactBuilding.CreateFactWithOr("�������� ��������� ������",
            linguisticsEdge, conlangEdge);
        // Todo: ���� ��� ����� ����� ��������� �����������, �� ��� ������� �� conlangEdge
        // (������ �� �����������). ���� ���� ������ ���� ����� - ����������� ������ ���
        facts.Add(languages);

        // �������� � ��������������� ������������ ����� ����������� � �����������
        // ����� ��������������, � ���, ��� ������� ����� - 20%
        var linguisticsPro = FactBuilding.CreateFactWithOr("����������� � �����������",
            new ProbabilityFactor(0.95f, linguisticsNode.Id),
            new ProbabilityFactor(0.2f, conlangHobby.Id)
            );
        facts.Add(linguisticsPro);

        // ���� �������� ������� � ������ ��������.
        // ���� ������� ���� ����������� ����� �� ���������� ���������
        Fact<string> raceNode = FactBuilding.CreateFact(1, "����", null);
        facts.Add(raceNode);
        facts.AddRange(FactBuilding.CreateAbstractFactVariants(raceNode,
            "���������", "���������", "�����������", "��������", "���������"));

        return facts;
    }
}
