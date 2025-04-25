using CausalModel.CLI.Enums;
using System.CommandLine;
using System.CommandLine.Binding;

namespace CausalModel.CLI.Options;

public class FixatorOptions
{
    public FixatorType FixatorType { get; set; }

    public static Option<FixatorType> CreateFixatorTypeOption()
    {
        return new Option<FixatorType>(
            aliases: new[] { "--fixator-type", "--fixator" },
            getDefaultValue: () => FixatorType.Default,
            description: "Type of fixator (Default/Pending)");
    }
}
