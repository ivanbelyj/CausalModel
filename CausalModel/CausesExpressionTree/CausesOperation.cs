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
    /// <summary>
    /// Представляет операцию логической группировки причинных связей узлов.
    /// Операция может принимать множество аргументов
    /// </summary>
    public abstract class CausesOperation : CausesExpression
    {
        public CausesOperation(IEnumerable<CausesExpression> operands)
        {
            Operands = operands.ToList();
        }

        public IEnumerable<CausesExpression> Operands { get; set; }
        public override IEnumerable<ProbabilityFactor> GetEdges()
        {
            List<ProbabilityFactor> edges = new List<ProbabilityFactor>();
            foreach (CausesExpression operand in Operands)
            {
                edges.AddRange(operand.GetEdges());
            }
            return edges;
        }

        public override bool? Evaluate<TFactValue>(
            IModelProvider<TFactValue> factProvider,
            IFixatedProvider happenedProvider,
            IRandomProvider fixatingValueProvider)
            => Operation(Operands.Select(expr => expr.Evaluate(factProvider,
                happenedProvider, fixatingValueProvider))
                .ToArray());

        protected abstract bool? Operation(bool?[] operands);
    }
}
