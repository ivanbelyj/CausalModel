using CausalModel.Facts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CausalModel.Serialization
{
    /// <summary>
    /// Causal model serialization structure
    /// </summary>
    internal class CausalModel<TFactValue>
        where TFactValue : class
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; } = null!;
        public List<Fact<TFactValue>> Facts { get; set; } = new List<Fact<TFactValue>>();
        public List<DeclaredBlock> DeclaredBlocks { get; set; } = new List<DeclaredBlock>();

        public CausalModel(string name)
        {
            Name = name;
        }

        // For deserialization only
        public CausalModel() : this(null!) { }
    }
}
