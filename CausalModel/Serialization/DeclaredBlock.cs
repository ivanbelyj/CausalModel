using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CausalModel.Serialization
{
    /// <summary>
    /// Declared block serialization structure
    /// </summary>
    internal class DeclaredBlock
    {
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        public string? Convention { get; set; }
        public string? CausesConvention { get; set; }

        public Dictionary<string, string>? BlockCausesMap { get; set; }
        public Dictionary<string, string>? BlockConsequencesMap { get; set; }

        public DeclaredBlock(string id)
        {
            Id = id;
        }

        // For deserialization only
        private DeclaredBlock() : this(null!) { }
    }
}
