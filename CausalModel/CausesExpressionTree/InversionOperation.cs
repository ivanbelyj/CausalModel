using CausalModel.Factors;
using CausalModel.Fixation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Model;

namespace CausalModel.CausesExpressionTree
{
    public class InversionOperation : CausesExpression
    {
        private CausesExpression expression;
        public InversionOperation(CausesExpression expression)
        {
            this.expression = expression;
        }

        public override IEnumerable<ProbabilityFactor> GetEdges() => expression.GetEdges();

        public override bool? Evaluate<TFactValue>(
            IModelProvider<TFactValue> factProvider,
            //IInstanceFactProvider<TFactValue> factProvider,
            IFixatedProvider happenedProvider, IRandomProvider fixingValueProvider)
        {
            bool? eval = expression.Evaluate(factProvider, happenedProvider, fixingValueProvider);
            if (eval == null)
                return null;
            else
                return !eval.Value;
        }
    }
}
