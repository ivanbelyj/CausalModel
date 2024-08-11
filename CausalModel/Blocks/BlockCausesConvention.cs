using CausalModel.Factors;
using CausalModel.Facts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks
{
    public class BlockCausesConvention
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Causes, required for the block in the external model.
        /// On the level of convention they are expressed by local ids (names)
        /// </summary>
        public List<string>? Causes { get; set; }

        public BlockCausesConvention(string name)
        {
            Name = name;
        }

        // Should be used for deserialization only
        private BlockCausesConvention() : this(null!)
        {

        }
    }
}