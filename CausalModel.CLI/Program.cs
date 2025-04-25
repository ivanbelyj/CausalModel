using CausalModel.CLI;
using CausalModel.CLI.Commands;
using CausalModel.CLI.Options;
using System.CommandLine;

ConfigureAnsiCodes.Configure();

var inputOption = CommonFixationOptions.CreateInputOption();
var seedOption = CommonFixationOptions.CreateSeedOption();
var fixatorTypeOption = FixatorOptions.CreateFixatorTypeOption();
var outputFormatOption = OutputOptions.CreateFormatOption();
var outputFormatIndentOption = OutputOptions.CreateFormatIndentOption();

var rootCommand = new RootCommand("CLI for Causal Model operations");
rootCommand.Name = "cm";

rootCommand.AddCommand(new FixateCommand().CreateCommand(inputOption, seedOption, fixatorTypeOption, outputFormatOption, outputFormatIndentOption));
rootCommand.AddCommand(new MonteCarloCommand().CreateCommand(inputOption, seedOption, fixatorTypeOption, outputFormatOption, outputFormatIndentOption));

return await rootCommand.InvokeAsync(args);
