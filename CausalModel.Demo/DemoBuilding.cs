using CausalModel.Factors;
using CausalModel.Facts;
using CausalModel.Model;
using CausalModel.Model.Blocks;
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
         BlockConventionMap<string> conventions)
    {
        return new ResolvingModelProvider<string>(
            model,
            new BlockResolver<string>(conventions));
    }

    public static BlockConventionMap<string> CreateDemoConventionMap()
    {
        return new BlockConventionMap<string>()
        {
            ModelsByConventionName = new Dictionary<string, CausalModel<string>>()
            {
                { "TestConvention", null! }
            }
        };
    }

    public static CausalModel<string> CreateDemoCausalModel()
    {
        var facts = CreateCharacterFacts();
        var causalModel = new CausalModel<string>()
        {
            Facts = facts,
            BlockConventions = new List<BlockConvention>()
            {
                new BlockConvention()
                {
                    Name = "TestConvention",
                    Causes = new()
                    {
                        new Factor()
                        {
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
                },
            },
            Blocks = new List<DeclaredBlock>()
            {
                new DeclaredBlock()
                {
                    Convention = "TestConvention",
                    Name = "block1"
                }
            }
        };
        return causalModel;
    }

    private static List<Fact<string>> CreateCharacterFacts()
    {
        // A simple example of a character model

        var facts = new List<Fact<string>>();
        Fact<string> hobbyRoot = FactsBuilding.CreateFact(0.9f, "�����");
        facts.Add(hobbyRoot);

        foreach (string hobbyName in new string[] { "���������",
            "������", "����������������", "Gamedev",
            "������������", "�����", "Role play",
            "3d �������������"})
        {
            facts.Add(FactsBuilding.CreateFact(0.3f, hobbyName, hobbyRoot.Id));
        }
        foreach (string hobbyName in new string[] { "Worldbuilding" })
        {
            facts.Add(FactsBuilding.CreateFact(0.1f, hobbyName, hobbyRoot.Id));
        }

        var educationNode = FactsBuilding.CreateFact(1, "�����������", null);
        facts.AddRange(FactsBuilding.CreateAbstractFact(educationNode,
            "������������ �����", "�������", "����������"));
        var linguisticsNode = FactsBuilding.CreateVariant(educationNode.Id,
            20, "�����������");
        facts.Add(linguisticsNode);

        var conlangHobby = FactsBuilding.CreateFact(0.1f, "�������� ������", hobbyRoot.Id);
        facts.Add(conlangHobby);

        // �������� ����, ��� �������� �������� ��������� ������, ����� ���� ���
        // �������� ������, ��� � �����������.
        // ������ ������ � ������ ������ �� ��������������
        var linguisticsEdge = new ProbabilityFactor(0.9f, linguisticsNode.Id);
        var conlangEdge = new ProbabilityFactor(0.3f, conlangHobby.Id);
        var languages = FactsBuilding.CreateFactWithOr("�������� ��������� ������",
            linguisticsEdge, conlangEdge);
        // Todo: ���� ��� ����� ����� ��������� �����������, �� ��� ������� �� conlangEdge
        // (������ �� �����������). ���� ���� ������ ���� ����� - ����������� ������ ���
        facts.Add(languages);

        // �������� � ��������������� ������������ ����� ����������� � �����������
        // ����� ��������������, � ���, ��� ������� ����� - 20%
        var linguisticsPro = FactsBuilding.CreateFactWithOr("����������� � �����������",
            new ProbabilityFactor(0.95f, linguisticsNode.Id),
            new ProbabilityFactor(0.2f, conlangHobby.Id)
            );
        facts.Add(linguisticsPro);

        // ���� �������� ������� � ������ ��������.
        // ���� ������� ���� ����������� ����� �� ���������� ���������
        Fact<string> raceNode = FactsBuilding.CreateFact(1, "����", null);
        facts.AddRange(FactsBuilding.CreateAbstractFact(raceNode,
            "���������", "���������", "�����������", "��������", "���������"));

        return facts;
    }

}
