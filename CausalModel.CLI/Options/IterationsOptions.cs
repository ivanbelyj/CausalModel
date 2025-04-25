using System.CommandLine;
using System.CommandLine.Binding;

namespace CausalModel.CLI.Options;

public static class IterationsOptions
{
    public static Option<int> CreateIterationsOption()
    {
        return new Option<int>(
            name: "--iterations",
            getDefaultValue: () => 10_000,
            description: "Number of iterations");
    }
}
