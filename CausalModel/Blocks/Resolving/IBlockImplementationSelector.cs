using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Blocks.Resolving
{
    public interface IBlockImplementationSelector
    {
        string GetImplementationModelName(DeclaredBlock declaredBlock);
    }
}
