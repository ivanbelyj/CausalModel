using CausalModel.Blocks.Resolving;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Serialization
{
    /// <summary>
    /// Serialization structure of data required for the fixation process
    /// </summary>
    internal class CausalBundle<TFactValue>
        where TFactValue : class
    {
        public IEnumerable<CausalModel<TFactValue>>? CausalModels { get; set; }
        public IEnumerable<BlockConvention>? BlockConventions { get; set; }
        public IEnumerable<BlockCausesConvention>? BlockCausesConventions { get; set; }

        public BlockResolvingMap? BlockResolvingMap { get; set; }

        public string? DefaultMainModel { get; set; }
    }
}
