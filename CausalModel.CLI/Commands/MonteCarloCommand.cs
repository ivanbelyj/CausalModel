using System.CommandLine;
using CausalModel.CLI.Enums;
using CausalModel.CLI.Options;
using CausalModel.CLI.Services;

namespace CausalModel.CLI.Commands;

public class MonteCarloCommand
{
    public Command CreateCommand(
        Option<string> inputOption,
        Option<int?> seedOption,
        Option<FixatorType> fixatorTypeOption,
        Option<OutputFormat> outputFormatOption,
        Option<bool> outputFormatIndentOption)
    {
        var iterationsOption = IterationsOptions.CreateIterationsOption();

        var command = new Command("montecarlo", "Run Monte Carlo simulation")
        {
            inputOption, seedOption, iterationsOption, fixatorTypeOption, outputFormatOption
        };

        command.SetHandler((
            string input,
            int? seed,
            int iterations,
            FixatorType fixatorType,
            OutputFormat outputFormat,
            bool writeIndented) =>
        {
            var service = new CausalFixationService(input);

            var result = service.RunMonteCarlo(seed, iterations, fixatorType, outputFormat, writeIndented);

            Console.WriteLine(result);
        },
        inputOption, seedOption, iterationsOption, fixatorTypeOption, outputFormatOption, outputFormatIndentOption);

        return command;
    }
}