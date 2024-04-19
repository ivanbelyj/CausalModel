using CausalModel.Fixation;
using CausalModel.Model.Instance;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Fixation.Fixators;

namespace CausalModel.Experiments;

// Will be removed
public class ExperimentsHelper
{
    private CausalGenerator<string>? generator;
    private PendingFixator<string> pendingFixator = new();
    private List<InstanceFactId> pendingFactIds = new();
    private CausalModel<string> model;
    private FixationFacadeBuilder<string>? facadeBuilder;

    public ExperimentsHelper(CausalModel<string> model)
    {
        this.model = model;
    }

    public void Run()
    {
        Initialize();

        Console.WriteLine("\nApproving pending facts");

        bool continueLoop = true;
        while (continueLoop)
        {
            ResetGenerator();
            FixateRoots();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ApproveAllPendingFacts();

            stopwatch.Stop();
            Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds}");

            continueLoop = AskUserToContinue();
        }
    }

    private void ApproveAllPendingFacts()
    {
        while (pendingFixator.HasPendingFacts())
        {
            pendingFixator.ApprovePendingFacts();
        }
    }

    private bool AskUserToContinue()
    {
        Console.WriteLine("Do you want to approve more facts? (y/n)");
        var userResponse = Console.ReadKey(true);
        return userResponse.KeyChar == 'y';
    }

    public void Initialize()
    {
        Console.WriteLine($"Model {model.Name}. Facts count: {model.Facts.Count}");

        pendingFixator.FactPending += (sender, instanceFact, isOccurred) => {
            pendingFactIds.Add(instanceFact.InstanceFactId);
            var fact = instanceFact.Fact;
            Console.WriteLine($"Pending fact: {fact.FactValue} ({fact.Id}). " +
                $"is occurred: {isOccurred}");
        };

        facadeBuilder = new FixationFacadeBuilder<string>(model)
            .WithFixator(pendingFixator)
            .AddOnFactFixated(OnFactFixated)
            .AddOnBlockImplemented((sender, block, convention, implementation) =>
            {
                Console.WriteLine($"// Block implemented: {block.Id}");
            })
            .AddOnModelInstanceCreated((sender, ModelInstance) =>
            {
                Console.WriteLine($"// Model instantiated: {ModelInstance.Model.Name} " +
                    $"{ModelInstance.InstanceId}");
            });
    }

    private void ResetGenerator()
    {
        if (facadeBuilder == null)
            throw new InvalidOperationException("Facade builder is not initialized");

        var fixationFacade = facadeBuilder.Build();
        generator = fixationFacade.CreateGenerator();
    }

    private void FixateRoots()
    {
        if (generator == null)
            throw new InvalidOperationException("Generator is not initialized. "
                + "Call Start method before generation");
        generator.FixateRootFacts();
    }

    private void OnFactFixated(
        object sender,
        InstanceFact<string> fixatedFact,
        bool isHappened)
    {
        if (isHappened)
        {
            string message = 
                (fixatedFact.Fact.IsRootCause() ? "" : "\t") + "Fact fixated: "
                + fixatedFact.Fact.FactValue + $" ({fixatedFact.InstanceFactId})";
            WriteColoredText(message, ConsoleColor.Green);
        }
    }

    private InstanceFact<string> GetFact(InstanceFactId factId)
        => generator!.ModelProvider.GetFact(factId.ToAddress());

    private void WriteColoredText(string str, ConsoleColor color)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(str);
        Console.ForegroundColor = prevColor;
    }
}
