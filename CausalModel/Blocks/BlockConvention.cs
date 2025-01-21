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
    public class BlockConvention
    {
        public string Name { get; set; }

        /// <summary>
        /// Consequences, that can be used by the external model.
        /// On the level of convention they are expressed by names only
        /// </summary>
        public List<string>? Consequences { get; set; }

        public BlockConvention(string name)
        {
            Name = name;
        }
    }
}