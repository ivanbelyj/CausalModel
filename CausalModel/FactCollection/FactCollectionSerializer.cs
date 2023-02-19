using CausalModel.Nodes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.FactCollection
{
    public class FactCollectionSerializer
    {
        public FactCollectionSerializer() { }
        // Полиморфная сериализация/десериализация необходима для
        // CausesExpression => And, Or, FactorLeaf, Not
        // (Гнезда - нет)
        // (Факторы - нет)
        // Fact => FactVariant

        public string ToJson<TNodeValue>(FactCollection<TNodeValue> factsCollection,
            bool writeIndented = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = writeIndented ? Formatting.Indented : Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new CausalModelSerializationBinder<TNodeValue>()
            };
            return JsonConvert.SerializeObject(factsCollection.Nodes, settings);
        }

        public FactCollection<TNodeValue>? FromJson<TNodeValue>(string jsonString)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new CausalModelSerializationBinder<TNodeValue>()
            };

            var facts = JsonConvert.DeserializeObject<List<Fact<TNodeValue>>>(
                jsonString, settings);
            if (facts == null)
                return null;
            var factsCollection = new FactCollection<TNodeValue>(facts);
            return factsCollection;
        }
    }
}
