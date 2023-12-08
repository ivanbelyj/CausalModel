using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts
{
    public class Fact<TNodeValue> : AbstractFact
    {
        private CausesExpression? causesExpression;
        public CausesExpression CausesExpression {
            get => causesExpression ?? GetDefaultCausesExpression();
            set => causesExpression = value;
        }
        public TNodeValue? NodeValue { get; set; }

        public string? AbstractFactId { get; set; }
        public IEnumerable<WeightFactor>? Weights { get; set; }

        private CausesExpression GetDefaultCausesExpression()
        {
            // The fact is a root cause by default
            return new FactorLeaf(new ProbabilityFactor(1, null));
        }

        public override string? ToString() =>
            $"{Id} - " + (NodeValue?.ToString() ?? "null");

        public override IEnumerable<Factor> GetCauses()
        {
            var res = new List<Factor>();
            if (CausesExpression != null)
                res.AddRange(CausesExpression.GetEdges());
            if (Weights != null)
                res.AddRange(Weights);

            return res;
        }
    }
}
