using CausalModel.Model.Serialization;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo;
public class FileHandler
{
    public CausalModel<string>? Deserialize(string fileName)
    {
        string fileContent = File.ReadAllText(fileName);
        var model = CausalModelSerialization.FromJson<string>(fileContent);
        return model;
    }

    public string Serialize(CausalModel<string> model,
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
