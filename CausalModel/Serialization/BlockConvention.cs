using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Serialization
{
    internal class BlockConvention
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }
        public List<string>? Consequences { get; set; }

        public BlockConvention(string name)
        {
            Name = name;
        }

        // For deserialization only
        private BlockConvention() : this(null!) { }
    }
}
