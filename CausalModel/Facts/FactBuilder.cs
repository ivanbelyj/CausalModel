using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts
{
    public class FactBuilder<TFactValue>
        where TFactValue : class
    {
        private CausesExpression? causesExpression;
        private TFactValue? nodeValue;
        private string? abstractFactId;
        private IEnumerable<WeightFactor>? weights;
        private string? id;

        public FactBuilder<TFactValue> WithCausesExpression(
            CausesExpression? causesExpression)
        {
            this.causesExpression = causesExpression;
            return this;
        }

        public FactBuilder<TFactValue> WithNodeValue(TFactValue? nodeValue)
        {
            this.nodeValue = nodeValue;
            return this;
        }

        public FactBuilder<TFactValue> WithAbstractFactId(string? abstractFactId)
        {
            this.abstractFactId = abstractFactId;
            return this;
        }

        public FactBuilder<TFactValue> WithWeights(IEnumerable<WeightFactor>? weights)
        {
            this.weights = weights;
            return this;
        }

        public FactBuilder<TFactValue> WithId(string? id)
        {
            this.id = id;
            return this;
        }

        public Fact<TFactValue> Build()
        {
            var res = new Fact<TFactValue>
            {
                FactValue = nodeValue,
                AbstractFactId = abstractFactId,
                Weights = weights,
            };

            if (id != null)
            {
                res.Id = id;
            }
            if (causesExpression != null)
            {
                res.CausesExpression = causesExpression;
            }

            return res;
        }
    }
}
