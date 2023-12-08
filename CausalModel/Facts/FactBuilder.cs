using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts;
public class FactBuilder<TNodeValue>
{
    private CausesExpression? causesExpression;
    private TNodeValue? nodeValue;
    private string? abstractFactId;
    private IEnumerable<WeightFactor>? weights;
    private string? id;

    public FactBuilder<TNodeValue> WithCausesExpression(
        CausesExpression? causesExpression)
    {
        this.causesExpression = causesExpression;
        return this;
    }

    public FactBuilder<TNodeValue> WithNodeValue(TNodeValue? nodeValue)
    {
        this.nodeValue = nodeValue;
        return this;
    }

    public FactBuilder<TNodeValue> WithAbstractFactId(string? abstractFactId)
    {
        this.abstractFactId = abstractFactId;
        return this;
    }

    public FactBuilder<TNodeValue> WithWeights(IEnumerable<WeightFactor>? weights)
    {
        this.weights = weights;
        return this;
    }

    public FactBuilder<TNodeValue> WithId(string? id)
    {
        this.id = id;
        return this;
    }

    public Fact<TNodeValue> Build()
    {
        var res = new Fact<TNodeValue>
        {
            CausesExpression = causesExpression,
            NodeValue = nodeValue,
            AbstractFactId = abstractFactId,
            Weights = weights
        };

        if (id != null) {
            res.Id = id;
        }

        return res;
    }
}
