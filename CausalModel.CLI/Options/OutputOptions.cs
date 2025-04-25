using CausalModel.CLI.Enums;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.CLI.Options;

public static class OutputOptions
{
    public static Option<OutputFormat> CreateFormatOption()
    {
        return new Option<OutputFormat>(
            aliases: new[] { "--output-format", "--format" },
            getDefaultValue: () => OutputFormat.HumanReadable,
            description: "Output format (Json/HumanReadable)");
    }

    public static Option<bool> CreateFormatIndentOption()
    {
        return new Option<bool>(
            aliases: new[] { "--write-indented", "--indented" },
            getDefaultValue: () => true,
            description: "Determines whether to indent the json output data");
    }
}