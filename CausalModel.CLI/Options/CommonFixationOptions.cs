using System.CommandLine;
using System.CommandLine.Binding;

namespace CausalModel.CLI.Options;

internal class CommonFixationOptions
{
    public string Input { get; set; }
    public int? Seed { get; set; }

    public static Option<string> CreateInputOption()
    {
        return new Option<string>(
            aliases: new[] { "--input", "-i" },
            description: "Input (file path or causal bundle raw JSON)")
        {
            IsRequired = true
        };
    }

    public static Option<int?> CreateSeedOption()
    {
        return new Option<int?>(
            aliases: new[] { "--seed", "-s" },
            description: "Random seed");
    }
}
