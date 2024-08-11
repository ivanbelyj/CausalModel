using CausalModel.Blocks;
using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts
{
    // Todo: rename ?
    /// <summary>
    /// Block declared in a causal model and implemented during generation
    /// </summary>
    public class BlockFact : FactWithCauses
    {
        private readonly BlockConvention? convention;

        public string? ConventionName => convention?.Name;

        public IEnumerable<Factor>? Causes => DeclaredBlock
            .CauseBlockReferencesMap
            .Values
            .Select(x => new Factor() { CauseId = x });

        public IEnumerable<BaseFact>? Consequences => convention?
            .Consequences
            .Select(x => new BaseFact() { Id = x });

        public DeclaredBlock DeclaredBlock { get; private set; }

        public BlockFact(DeclaredBlock declaredBlock, BlockConvention? convention)
        {
            DeclaredBlock = declaredBlock;
            this.convention = convention;
        }

        public override IEnumerable<Factor> GetCauses()
        {
            return Causes ?? new Factor[0];
        }
    }
}
