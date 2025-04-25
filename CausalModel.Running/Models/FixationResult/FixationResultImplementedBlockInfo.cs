using CausalModel.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Running.Models.FixationResult
{
    public class FixationResultImplementedBlockInfo
    {
        public string Id { get; private set; }
        public string Convention { get; private set; }
        public string CausesConvention { get; private set; }

        public FixationResultImplementedBlockInfo(DeclaredBlock declaredBlock)
        {
            Id = declaredBlock.Id;
            Convention = declaredBlock.Convention;
            CausesConvention = declaredBlock.CausesConvention;
        }
    }
}
