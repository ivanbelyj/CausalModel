using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using CausalModel.Nests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts
{
    public class Fact<TNodeValue> : AbstractFact
    {
        public CausesExpression? CausesExpression { get; set; }
        public TNodeValue? NodeValue { get; set; }

        public string? AbstractFactId { get; set; }
        public IEnumerable<WeightFactor>? Weights { get; set; }

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
