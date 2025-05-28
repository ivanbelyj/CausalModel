using CausalModel.Serialization.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Serialization
{
    public class SerializationUtils
    {
        public static string ToJson<TFactValue>(
            Common.CausalBundle<TFactValue> model,
            bool writeIndented = false)
            where TFactValue : class
        {
            var modelToSerialize = MappingUtils.Map(model);

            var settings = CreateSerializerSettings<TFactValue>(writeIndented);

            return JsonConvert.SerializeObject(modelToSerialize, settings);
        }

        public static Common.CausalBundle<TFactValue>? FromJson<TFactValue>(
            string jsonString)
            where TFactValue : class
        {
            var settings = CreateSerializerSettings<TFactValue>();

            var deserializedBundle = JsonConvert.DeserializeObject<CausalBundle<TFactValue>>(
                jsonString,
                settings);
            return deserializedBundle == null ? null : MappingUtils.Map(deserializedBundle);
        }

        public static JsonSerializerSettings CreateCommonSerializerSettings(
            bool writeIndented = false)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = writeIndented ? Formatting.Indented : Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            return settings;
        }

        private static JsonSerializerSettings CreateSerializerSettings<TFactValue>(
            bool writeIndented = false)
        {
            JsonSerializerSettings settings = CreateCommonSerializerSettings(writeIndented);
            settings.SerializationBinder = new CausalModelSerializationBinder<TFactValue>();

            return settings;
        }
    }
}
