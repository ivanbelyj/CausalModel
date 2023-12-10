using CausalModel.Fixation;
using CausalModel.Facts;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Model.Serialization;
using CausalModel.Model.Providers;
using CausalModel.Model.Blocks;

var rootCommand = new RootCommand
{
    new Option<FileInfo>(
        aliases: new[] { "--input", "--i" },
        description: "The input file with causal model facts to process.")
    {
        IsRequired = true,
    },
    new Option<FileInfo?>(
        aliases: new[] { "--output", "--o" },
        description: "The output file to write the results to."),
    new Option<int?>(
        aliases: new[] { "--seed", "--s" },
        description: "Specify a seed for generation."),
    new Option<bool?>(
        aliases: new[] { "--not-wait-for-read-key", "--nw" },
        description: "Determines whether the application should wait "
            + "for the button to be pressed after shutting down.")
};

rootCommand.Description = "Console utility for working with CausalModel";
rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo?, int?, bool?>(Run);

return await rootCommand.InvokeAsync(args);

static void Run(FileInfo input, FileInfo? output, int? seed, bool? notWaitForReadKey)
{
    if (input.Extension == ".cm" || input.Extension == ".json")
    {
        Console.WriteLine($"Processing file: {input.FullName}");

        ResolvingModelProvider<string> modelProvider = null!;
        Exception? exceptionToExit = null;

        // Todo: blocks resolving CLI support
        BlockConventionMap<string> conventions = new BlockConventionMap<string>()
        {
            ModelsByConventionName = new()
            {
                // Todo:
            }
        };
        BlockImplementationResolver<string> resolver = new BlockImplementationResolver<string>(conventions);

        try
        {
            var deserializationRes = Deserialize(input.FullName);
            if (deserializationRes == null)
                throw new NullReferenceException(input.FullName);
            else
                modelProvider = new ResolvingModelProvider<string>(deserializationRes,
                    resolver);

        } catch (NullReferenceException ex)
        {
            Console.WriteLine("The input file deserialization result is null. ");
            exceptionToExit = ex;
            
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error while reading the input file. ");
            exceptionToExit = ex;
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while deserializing the input file. ");
            exceptionToExit = ex;
        }

        if (exceptionToExit != null)
        {
            Console.WriteLine(exceptionToExit);
            WaitForReadKey();
            return;
        }

        // Generate facts
        string genOutput = Generate(modelProvider, seed);

        // If an output file is specified, write the results to the file
        if (output != null)
        {
            File.WriteAllText(output.FullName, genOutput);
            Console.WriteLine($"Results written to: {output.FullName}");
        }
    }
    else
    {
        Console.WriteLine("Invalid input file. Please provide a .cm or .json file "
            + "containing causal model facts. ");
    }

    WaitForReadKey();

    void WaitForReadKey()
    {
        if (notWaitForReadKey == null || !notWaitForReadKey.Value)
        {
            Console.ReadKey();
        }
    }
}

static CausalModel.Model.CausalModel<string>? Deserialize(string fileName)
{
    string fileContent = File.ReadAllText(fileName);
    var model = CausalModelSerialization.FromJson<string>(fileContent);
    return model;
}

//static string Serialize(FactCollection<string> factCollection,
//    string fileName)
//{
//    string jsonString = FactCollectionUtils.Serialize(factCollection, fileName);
//    if (!fileName.EndsWith(".json"))
//    {
//        fileName += ".json";
//    }
//    File.WriteAllText(fileName, jsonString);
//    return jsonString;
//}

static string Generate(ResolvingModelProvider<string> model, int? seed = null)
{
    if (seed == null)
        seed = new Random().Next();

    Console.WriteLine("Seed: " + seed);
    Fixator<string> fixator = new Fixator<string>();
    var generator = new CausalGenerator<string>(model, fixator, seed.Value);

    var resStringBuilder = new StringBuilder();
    resStringBuilder.AppendLine("Seed: " + seed);
    fixator.FactFixated += (object sender, Fact<string> fact, bool isHappened) =>
    {
        if (isHappened)
        {
            string newLine = (fact.IsRootCause() ? "" : "\t") + fact.FactValue;
            Console.WriteLine(newLine);
            resStringBuilder.AppendLine(newLine);
        }
    };

    generator.FixateRootCauses();
    return resStringBuilder.ToString();
}
