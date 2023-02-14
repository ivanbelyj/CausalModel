using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.CausesExpressionTree
{
    public class DisjunctionOperation : CausesOperation
    {
        public DisjunctionOperation(IEnumerable<CausesExpression> operands) : base(operands) { }

        protected override bool Operation(bool[] operands)
        {
            foreach (bool operand in operands)
            {
                if (operand)
                    return true;
            }
            return false;
        }
    }
}
