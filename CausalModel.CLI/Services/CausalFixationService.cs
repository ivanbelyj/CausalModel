using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CausalModel.CLI.Enums;
using CausalModel.Common;
using CausalModel.Common.DataProviders;
using CausalModel.Fixation;
using CausalModel.Fixation.Fixators;
using CausalModel.Fixation.Fixators.Pending;
using CausalModel.Model.Instance;
using CausalModel.Running;
using CausalModel.Running.ProbabilityEstimation;
using CausalModel.Serialization;
using CausalModel.Running.Models;
using Newtonsoft.Json;

namespace CausalModel.CLI.Services;

internal class CausalFixationService
{
    private readonly CausalBundle<string> bundle;

    public CausalFixationService(string input)
    {
        bundle = ParseInput(input) ?? throw new ArgumentException(nameof(input), "Input serialization result is null");
    }

    public string Fixate(int? seed, FixatorType fixatorType, OutputFormat outputFormat, bool writeIndented)
    {
        var result = new CausalFixationHelper(bundle).Fixate(seed, fixatorType);
        return outputFormat switch
        {
            OutputFormat.HumanReadable => result.ToString(),
            OutputFormat.Json => JsonConvert.SerializeObject(
                result,
                SerializationUtils.CreateCommonSerializerSettings(writeIndented)),
            _ => throw new ArgumentException(nameof(outputFormat), $"Unsupported {nameof(OutputFormat)} value"),
        };
    }

    public string RunMonteCarlo(
        int? seed,
        int iterations,
        FixatorType fixatorType,
        OutputFormat outputFormat,
        bool writeIndented)
    {
        var facadeBuilder = new FixationFacadeBuilder<string>(bundle);
        var simulationsRunner = new ProbabilityEstimator<string>(facadeBuilder);

        var totalResult = simulationsRunner.RunSimulations(iterations);

        return outputFormat switch
        {
            OutputFormat.HumanReadable => totalResult.ToString(),
            OutputFormat.Json => JsonConvert.SerializeObject(
                totalResult,
                SerializationUtils.CreateCommonSerializerSettings(writeIndented)),
            _ => throw new ArgumentException(nameof(outputFormat), $"Unsupported {nameof(OutputFormat)} value"),
        };
    }

    private static CausalBundle<string>? ParseInput(string input)
    {
        return Deserialize(File.Exists(input) ? File.ReadAllText(input) : input);
    }

    private static CausalBundle<string>? Deserialize(string json)
    {
        return SerializationUtils.FromJson<string>(json);
    }
}
