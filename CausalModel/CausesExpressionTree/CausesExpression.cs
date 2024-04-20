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
    /// Представляет логическое выражение группировки причин.
    /// </summary>
    public abstract class CausesExpression
    {
        public abstract bool? Evaluate<TFactValue>(
            IModelProvider<TFactValue> factProvider,
            IFixatedProvider happenedProvider,
            IRandomProvider fixingValueProvider)
            where TFactValue : class;
        // Todo: shouldn't we make the class generic?

        /// <summary>
        /// Ребра, включенные в логическое выражение
        /// </summary>
        public abstract IEnumerable<ProbabilityFactor> GetEdges();
    }
}
