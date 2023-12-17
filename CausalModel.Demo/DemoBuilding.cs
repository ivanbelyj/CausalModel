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
    public static ResolvedModelProvider<string> CreateDemoModelProvider()
    {
        return CreateModelProvider(CreateDemoCausalModel(), CreateDemoConventionMap());
    }

    public static ResolvedModelProvider<string> CreateModelProvider(
         CausalModel<string> model, 
         BlockResolvingMap<string> conventions)
    {
        var modelInstanceFactory = new ModelInstanceFactory<string>();
        modelInstanceFactory.ModelInstanceCreated += (sender, ModelInstance) =>
        {
            Console.WriteLine($"// Model instanced: {ModelInstance.InstanceId}");
        };

        var resolver = new BlockResolver<string>(conventions, modelInstanceFactory);
        resolver.BlockImplemented += (sender, block, convention, implementation) =>
        {
            Console.WriteLine($"// Block implemented: {block.Id}");
        };

        var modelInstance = modelInstanceFactory.InstantiateModel(model);
        
        return new ResolvedModelProvider<string>(modelInstance, resolver);
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
        facts.Add(FactsBuilding.CreateFact(
            0.9f,
            value: "Block cause",
            causeId: null,
            id: "BlockCause"));

        // Add fact using block consequence
        facts.Add(FactsBuilding.CreateFact(1f,
            value: "Fact using block consequence",
            causeId: "BlockConsequence"));

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
                FactsBuilding.CreateFact(1,
                    "Block consequence (can be used in the parent model)",
                    fact2.Id,
                    "BlockConsequence"),
                //FactsBuilding.CreateFact(
                //    probability: 1,
                //    value: "! Fact using external cause",
                //    causeId: "BlockCause"),
            }
        };
        return (conv, impl);
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
