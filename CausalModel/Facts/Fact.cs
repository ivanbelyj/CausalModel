using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts
{
    public class Fact<TFactValue> : FactWithCauses
        where TFactValue : class
    {
        public CausesExpression CausesExpression { get; set; }

        public TFactValue? FactValue { get; set; }

        public string? AbstractFactId { get; set; }
        public IEnumerable<WeightFactor>? Weights { get; set; }

        public Fact()
        {
            CausesExpression = GetDefaultCausesExpression();
        }

        private CausesExpression GetDefaultCausesExpression()
        {
            // The fact is a root cause by default
            return new FactorLeaf(new ProbabilityFactor(1, null));
        }

        public override string? ToString() =>
            $"{Id} - " + (FactValue?.ToString() ?? "null");

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
