using CausalModel.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Common
{
    public class ConventionsProvider : IConventionsProvider
    {
        public BlockCausesConvention GetCauseConventionByName(string convName)
        {
            throw new NotImplementedException();
        }

        public BlockConvention GetConventionByName(string convName)
        {
            throw new NotImplementedException();
        }
    }
}
