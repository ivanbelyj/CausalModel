using CausalModel.Facts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Serialization
{
    public class CausalModelSerialization
    {
        public static string ToJson<TFactValue>(CausalModel<TFactValue> model,
            bool writeIndented = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = writeIndented ? Formatting.Indented : Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new CausalModelSerializationBinder<TFactValue>()
            };
            return JsonConvert.SerializeObject(model, settings);
        }

        public static CausalModel<TFactValue>? FromJson<TFactValue>(string jsonString)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new CausalModelSerializationBinder<TFactValue>()
            };

            var model = JsonConvert.DeserializeObject<CausalModel<TFactValue>>(
                jsonString, settings);
            return model;
        }
    }
}
