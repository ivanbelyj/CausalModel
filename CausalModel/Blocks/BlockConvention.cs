using CausalModel.Blocks.BlockReferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks;
/// <summary>
/// The block defines the convention (block convention) - a set of inputs and outputs,
/// references (block references). Inputs refer to external facts for the block,
/// outputs become factors for facts that follow from the block.
/// Any CM can be the potential implementation of the block (block implementation),
/// satisfying its convention.
/// </summary>
public class BlockConvention
{
    public string Name { get; set; }
	public IEnumerable<BlockReference>? Causes { get; set; }
    public IEnumerable<BlockReference>? Consequences { get; set; }
}
