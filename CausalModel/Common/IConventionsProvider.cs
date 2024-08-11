using CausalModel.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Common
{
    public interface IConventionsProvider
    {
        BlockConvention GetConventionByName(string convName);
        BlockCausesConvention GetCauseConventionByName(string convName);
    }
}
