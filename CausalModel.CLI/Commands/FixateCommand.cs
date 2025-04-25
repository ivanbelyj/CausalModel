using System.CommandLine;
using CausalModel.CLI.Enums;
using CausalModel.CLI.Options;
using CausalModel.CLI.Services;

namespace CausalModel.CLI.Commands;

public class FixateCommand
{
    public Command CreateCommand(
        Option<string> inputOption,
        Option<int?> seedOption,
        Option<FixatorType> fixatorTypeOption,
        Option<OutputFormat> outputFormatOption,
        Option<bool> outputFormatIndentOption)
    {
        var command = new Command("fixate", "Fixate a causal model")
        {
            inputOption, seedOption, fixatorTypeOption, outputFormatOption, outputFormatIndentOption
        };

        command.SetHandler((
            string input,
            int? seed,
            FixatorType fixatorType,
            OutputFormat outputFormat,
            bool writeIndented) =>
        {
            var service = new CausalFixationService(input);

            var result = service.Fixate(
                seed,
                fixatorType,
                outputFormat,
                writeIndented);

            Console.WriteLine(result);
        },
        inputOption, seedOption, fixatorTypeOption, outputFormatOption, outputFormatIndentOption);

        return command;
    }
}