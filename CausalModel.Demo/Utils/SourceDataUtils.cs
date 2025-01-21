using CausalModel.Blocks.Resolving;
using CausalModel.Common;
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

    public static CausalBundle<string> GetFromFileOrDefault(string fileName)
    {
        //CausalModel<string> causalModel;
        CausalBundle<string> bundle;

        if (/*!CREATE_FILE && */ File.Exists(fileName))
        {
            Console.WriteLine("Found " + fileName);
            bundle = FileUtils.Deserialize(fileName)
                ?? throw new NullReferenceException("Deserialized model is null");
        }
        else if (fileName != DefaultFactsFile && File.Exists(DefaultFactsFile))
        {
            Console.WriteLine($"File {fileName} not found. Found {DefaultFactsFile}, use example");
            fileName = DefaultFactsFile;
            bundle = FileUtils.Deserialize(fileName)
                ?? throw new NullReferenceException("Deserialized model is null");
        }
        else
        {
            Console.WriteLine($"File {fileName} not found. Create {DefaultFactsFile}");

            bundle = DemoBundleBuildingUtils.CreateDemoCausalBundle();

            FileUtils.Serialize(bundle, fileName);
            Console.WriteLine("Data used for generation saved to " + fileName);
        }

        return bundle;
    }
}
