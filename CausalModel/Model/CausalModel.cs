using CausalModel.Blocks;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model
{
    /// <summary>
    /// Represents not resolved causal model, including declared blocks
    /// </summary>
    public class CausalModel<TFactValue>
        where TFactValue : class
    {
        public string Name { get; set; }

        /// <summary>
        /// The facts of the causal model, not including facts from blocks
        /// </summary>
        public List<Fact<TFactValue>> Facts { get; set; } = new List<Fact<TFactValue>>();

        public List<DeclaredBlock> DeclaredBlocks { get; set; } = new List<DeclaredBlock>();

        public CausalModel(string name)
        {
            Name = name;
        }
    }
}
