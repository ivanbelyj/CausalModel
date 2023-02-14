using CausalModel.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.CausesExpressionTree
{
    public class InversionOperation : CausesExpression
    {
        private CausesExpression expression;
        public InversionOperation(CausesExpression expression)
        {
            this.expression = expression;
        }

        public override IEnumerable<ProbabilityEdge> GetEdges() => expression.GetEdges();

        public override bool Evaluate() => !expression.Evaluate();
    }
}
