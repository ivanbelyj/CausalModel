using CausalModel.Factors;
using CausalModel.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts;

/// <summary>
/// Block declared in a causal model and implemented during generation
/// </summary>
public class BlockFact : FactWithCauses
{
    private readonly BlockConvention convention;

    public string ConventionName => convention.Name;
    public IEnumerable<Factor>? Causes => convention.Causes;
    public IEnumerable<BaseFact>? Consequences => convention.Consequences;

    public BlockFact(BlockConvention convention)
    {
        this.convention = convention;
    }

    public override IEnumerable<Factor> GetCauses()
    {
        return Causes ?? new Factor[0];
    }
}
