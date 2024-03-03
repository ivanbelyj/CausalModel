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

        protected override bool? Operation(bool?[] operands)
        {
            bool? operand = null;
            bool returnNull = false;
            for (int i = 0; i < operands.Length; i++)
            {
                operand = operands[i];
                if (operand != null && operand.Value)
                    return true;
                if (operand == null)
                {
                    returnNull = true;
                }
            }

            if (returnNull)
                return null;
            else return false;
        }
    }
}
