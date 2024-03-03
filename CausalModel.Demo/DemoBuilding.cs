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

namespace CausalModel.Demo;
public class DemoBuilding
{
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

    private static BlockConvention CreateDemoConvention()
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

        return conv;
    }

    private static CausalModel<string> CreateDemoConventionImplementation()
    {
        var fact1 = FactBuilding.CreateFact(1, "Inner fact 1", null);
        var fact2 = FactBuilding.CreateFact(1, "Inner fact 2", fact1.Id);
        var impl = new CausalModel<string>()
        {
            Facts = new()
            {
                fact1,
                fact2,
                FactBuilding.CreateFact(1, "Inner fact 3", null),
                FactBuilding.CreateFact(1,
                    "Block consequence (can be used in the parent model)",
                    fact2.Id,
                    "BlockConsequence"),
                FactBuilding.CreateFact(
                    probability: 1,
                    value: "! Fact using external cause",
                    causeId: "BlockCause"),
            }
        };
        impl.Name = "Test model (used as block)";
        return impl;
    }

    public static CausalModel<string> CreateDemoCausalModel()
    {
        var facts = CreateCharacterFacts();

        // Required for the declared block
        facts.Add(FactBuilding.CreateFact(
            0.9f,
            value: "Block cause",
            causeId: null,
            id: "BlockCause"));

        // Add fact using block consequence
        facts.Add(FactBuilding.CreateFact(1f,
            value: "Fact using block consequence",
            causeId: "BlockConsequence"));

        var causalModel = new CausalModel<string>()
        {
            Facts = facts,
            BlockConventions = new List<BlockConvention>()
            {
                CreateDemoConvention()
            },
            Blocks = new List<DeclaredBlock>()
            {
                new DeclaredBlock("block1", "TestConvention")
            }
        };
        causalModel.Name = "Character Model";
        return causalModel;
    }

    private static List<Fact<string>> CreateCharacterFacts()
    {
        // A simple example of a character model

        var facts = new List<Fact<string>>();
        Fact<string> hobbyRoot = FactBuilding.CreateFact(0.9f, "�����");
        facts.Add(hobbyRoot);

        foreach (string hobbyName in new string[] { "���������",
            "������", "����������������", "Gamedev",
            "������������", "�����", "Role play",
            "3d �������������"})
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
