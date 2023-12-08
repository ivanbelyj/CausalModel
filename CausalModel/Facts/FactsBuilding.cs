using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using CausalModel.Nests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts;

public static class FactUtils
{
    public static Fact<TNodeValue> CreateNode<TNodeValue>(
        float probability, TNodeValue? value = default, string? causeId = null)
    {
        return new FactBuilder<TNodeValue>()
            .WithCausesExpression(new FactorLeaf(
                new ProbabilityFactor(probability, causeId)))
            .WithNodeValue(value)
            .Build();
    }

    private static Fact<TNodeValue> CreateNodeWithOperation<TNodeValue>(
        TNodeValue value, ProbabilityFactor[] edges,
        Func<ProbabilityFactor[], CausesOperation> operation)
    {
        return new FactBuilder<TNodeValue>()
            .WithNodeValue(value)
            .WithCausesExpression(operation(edges))
            .Build();
    }

    public static Fact<TNodeValue> CreateNodeWithAnd<TNodeValue>(TNodeValue value,
        params ProbabilityFactor[] edges)
    {
        return CreateNodeWithOperation(value, edges, Expressions.And);
    }

    public static Fact<TNodeValue> CreateNodeWithOr<TNodeValue>(TNodeValue value,
        params ProbabilityFactor[] edges)
    {
        return CreateNodeWithOperation(value, edges, Expressions.Or);
    }

    private static Fact<TNodeValue> CreateVariant<TNodeValue>(
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

