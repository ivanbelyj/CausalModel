using CausalModel.Blocks.Resolving;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CausalModel.Demo.Utils.WriteUtils;

namespace CausalModel.Demo.Utils;
internal class SourceDataUtils
{
    private const string DefaultFactsFile = "character-facts.json";

    public static SourceData GetFromFileOrDefault(string fileName)
    {
        CausalModel<string> causalModel;

        if (/*!CREATE_FILE && */ File.Exists(fileName))
        {
            Console.WriteLine("Found " + fileName);
            causalModel = FileUtils.Deserialize(fileName)
                ?? throw new NullReferenceException("Deserialized model is null");
        }
        else if (fileName != DefaultFactsFile && File.Exists(DefaultFactsFile))
        {
            Console.WriteLine($"File {fileName} not found. Found {DefaultFactsFile}, use example");
            fileName = DefaultFactsFile;
            causalModel = FileUtils.Deserialize(fileName)
                ?? throw new NullReferenceException("Deserialized model is null");
        }
        else
        {
            Console.WriteLine($"File {fileName} not found. Create {DefaultFactsFile}");

            causalModel = BuildingUtils.CreateDemoCausalModel();

            FileUtils.Serialize(causalModel, fileName);
            Console.WriteLine("Data used for generation saved to " + fileName);
        }

        // Todo: get it from file ?
        var blockResolvingMap = BuildingUtils.CreateDemoConventionMap();

        return new SourceData(causalModel, blockResolvingMap);
    }
}
