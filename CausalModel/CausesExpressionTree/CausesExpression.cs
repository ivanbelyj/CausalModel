using CausalModel.Factors;
using CausalModel.Fixation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Model.Providers;

namespace CausalModel.CausesExpressionTree
{
    /// <summary>
    /// Представляет логическое выражение группировки причин.
    /// </summary>
    public abstract class CausesExpression
    {
        public abstract bool? Evaluate<TFactValue>(
            IFactProvider<TFactValue> factProvider,
            IFixatedProvider happenedProvider, IRandomProvider fixingValueProvider);

        /// <summary>
        /// Ребра, включенные в логическое выражение
        /// </summary>
        public abstract IEnumerable<ProbabilityFactor> GetEdges();
    }
}
