using CausalModel.Factors;
using CausalModel.Fixation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Model;
using CausalModel.Fixation.Random;

namespace CausalModel.CausesExpressionTree
{
    public class InversionOperation : CausesExpression
    {
        public CausesExpression CausesExpression { get; set; }

        public InversionOperation(CausesExpression expression)
        {
            CausesExpression = expression;
        }

        public override IEnumerable<ProbabilityFactor> GetEdges() => CausesExpression.GetEdges();

        public override bool? Evaluate<TFactValue>(
            IModelProvider<TFactValue> factProvider,
            IFixationStateProvider happenedProvider,
            IRandomProvider fixingValueProvider)
        {
            bool? eval = CausesExpression.Evaluate(factProvider, happenedProvider, fixingValueProvider);
            if (eval == null)
                return null;
            else
                return !eval.Value;
        }
    }
}
