using CausalModel.Factors;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks.Resolving;

public class BlockResolvingException : Exception
{
    public List<Factor>? NotImplementedCauses { get; set; }
    public List<BaseFact>? NotImplementedConsequences { get; set; }

    public BlockResolvingException(string? message)
        : base(message ?? $"Failed to resolve block convention. ")
    {
    }

    public BlockResolvingException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
