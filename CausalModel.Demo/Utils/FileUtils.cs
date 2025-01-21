using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CausalModel.Serialization;
using CausalModel.Common;

namespace CausalModel.Demo.Utils;
internal static class FileUtils
{
    private const string FACTS_FILE = "character-facts.json";
    public static string GetFileName()
    {
        Console.WriteLine($"Enter file name or press Enter to use or create {FACTS_FILE} (example)");
        string? fileName = Console.ReadLine();
        if (string.IsNullOrEmpty(fileName))
            fileName = FACTS_FILE;

        return fileName;
    }

    public static CausalBundle<string>? Deserialize(string fileName)
    {
        string fileContent = File.ReadAllText(fileName);
        var bundle = SerializationUtils.FromJson<string>(fileContent);
        return bundle;
    }

    public static string Serialize(
        CausalBundle<string> bundle,
        string fileName = "fact-collection.json")
    {
        string jsonString = SerializationUtils.ToJson(bundle, true);
        if (!fileName.EndsWith(".json"))
        {
            fileName += ".json";
        }
        File.WriteAllText(fileName, jsonString);
        return jsonString;
    }
}
