using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CausalModel.Common
{
    /// <summary>
    /// All initial data necessary for the fixation process. Usually deserialized and mapped
    /// from a file
    /// </summary>
    public class CausalBundle<TFactValue>
        where TFactValue : class
    {
        public IEnumerable<CausalModel<TFactValue>> CausalModels {
            get;
            set;
        } = new List<CausalModel<TFactValue>>();

        public IEnumerable<BlockConvention> BlockConventions {
            get;
            set;
        } = new List<BlockConvention>();

        public IEnumerable<BlockCausesConvention> BlockCausesConventions {
            get;
            set;
        } = new List<BlockCausesConvention>();

        public BlockResolvingMap BlockResolvingMap { get; set; } = new BlockResolvingMap();

        public string? DefaultMainModel { get; set; }
    }
}
