using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.FactCollection;
public static class FactCollectionUtils
{
    public static FactCollection<string>? Deserialize(string jsonString)
    {
        var serializer = new FactCollectionSerializer();
        FactCollection<string>? factCol = serializer.FromJson<string>(jsonString);
        return factCol;
    }

    public static string Serialize(FactCollection<string> factCollection,
        string fileName)
    {
        var serializer = new FactCollectionSerializer();
        string jsonString = serializer.ToJson(factCollection, true);
        return jsonString;
    }
}
