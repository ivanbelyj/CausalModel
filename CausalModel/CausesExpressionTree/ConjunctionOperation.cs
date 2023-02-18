﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.CausesExpressionTree
{
    public class ConjunctionOperation : CausesOperation
    {
        public ConjunctionOperation(IEnumerable<CausesExpression> operands) : base(operands) { }

        protected override bool? Operation(bool?[] operands)
        {
            bool? operand = null;
            bool returnNull = false;
            for (int i = 0; i < operands.Length; i++)
            {
                operand = operands[i];
                if (operand != null && !operand.Value)
                    return false;
                if (operand == null)
                {
                    returnNull = true;
                }
            }
            return returnNull ? null : true;
        }
    }
}
