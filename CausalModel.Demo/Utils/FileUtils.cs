using CausalModel.Model.Serialization;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

    public static CausalModel<string>? Deserialize(string fileName)
    {
        string fileContent = File.ReadAllText(fileName);
        var model = CausalModelSerialization.FromJson<string>(fileContent);
        return model;
    }

    public static string Serialize(CausalModel<string> model,
        string fileName = "fact-collection.json")
    {
        string jsonString = CausalModelSerialization.ToJson<string>(model, true);
        if (!fileName.EndsWith(".json"))
        {
            fileName += ".json";
        }
        File.WriteAllText(fileName, jsonString);
        return jsonString;
    }
}
