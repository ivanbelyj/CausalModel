using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Serialization
{
    internal class BlockCausesConvention
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }
        public List<string>? Causes { get; set; }

        public BlockCausesConvention(string name)
        {
            Name = name;
        }

        // For deserialization only
        private BlockCausesConvention() : this(null!) { }
    }
}
