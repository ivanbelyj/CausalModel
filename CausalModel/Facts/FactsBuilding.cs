using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts;

public static class FactsBuilding
{
    public static Fact<TNodeValue> CreateFact<TNodeValue>(
        float probability, TNodeValue? value = default, string? causeId = null)
    {
        return new FactBuilder<TNodeValue>()
            .WithCausesExpression(new FactorLeaf(
                new ProbabilityFactor(probability, causeId)))
            .WithNodeValue(value)
            .Build();
    }

    private static Fact<TNodeValue> CreateFactWithOperation<TNodeValue>(
        TNodeValue value, ProbabilityFactor[] edges,
        Func<ProbabilityFactor[], CausesOperation> operation)
    {
        return new FactBuilder<TNodeValue>()
            .WithNodeValue(value)
            .WithCausesExpression(operation(edges))
            .Build();
    }

    public static Fact<TNodeValue> CreateFactWithAnd<TNodeValue>(TNodeValue value,
        params ProbabilityFactor[] edges)
    {
        return CreateFactWithOperation(value, edges, Expressions.And);
    }

    public static Fact<TNodeValue> CreateFactWithOr<TNodeValue>(TNodeValue value,
        params ProbabilityFactor[] edges)
    {
        return CreateFactWithOperation(value, edges, Expressions.Or);
    }

    public static Fact<TNodeValue> CreateVariant<TNodeValue>(
        string abstractNodeId, float weight, TNodeValue? value = default)
    {
        return new FactBuilder<TNodeValue>()
            .WithAbstractFactId(abstractNodeId)
            .WithWeights(new List<WeightFactor> {
                new WeightFactor(weight)
            })
            .WithNodeValue(value)
            .Build();
    }

    public static List<Fact<TNodeValue>> CreateAbstractFact<TNodeValue>(
        Fact<TNodeValue> abstractFact, params TNodeValue[] variants)
    {
        var res = new List<Fact<TNodeValue>>() { abstractFact };

        foreach (var val in variants)
        {
            var node = CreateVariant(abstractFact.Id, 1, val);
            res.Add(node);
        }

        return res;
    }
}
