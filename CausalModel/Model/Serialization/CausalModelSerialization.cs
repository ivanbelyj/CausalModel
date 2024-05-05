using CausalModel.Facts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Serialization
{
    public class CausalModelSerialization
    {
        public static string ToJson<TFactValue>(
            CausalModel<TFactValue> model,
            bool writeIndented = false)
            where TFactValue : class
        {
            var settings = CreateSerializerSettings<TFactValue>(writeIndented);

            return JsonConvert.SerializeObject(model, settings);
        }

        public static CausalModel<TFactValue>? FromJson<TFactValue>(
            string jsonString)
            where TFactValue : class
        {
            var settings = CreateSerializerSettings<TFactValue>();

            var model = JsonConvert
                .DeserializeObject<CausalModel<TFactValue>>(
                    jsonString, settings);
            return model;
        }

        private static JsonSerializerSettings CreateSerializerSettings<TFactValue>(
            bool writeIndented = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = writeIndented ? Formatting.Indented : Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new CausalModelSerializationBinder<TFactValue>(),
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            return settings;
        }
    }
}
