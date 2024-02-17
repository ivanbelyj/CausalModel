using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts;

public static class FactBuilding
{
    public static Fact<TFactValue> CreateFact<TFactValue>(
        float probability, TFactValue? value = default, string? causeId = null,
        string? id = null)
    {
        return new FactBuilder<TFactValue>()
            .WithCausesExpression(new FactorLeaf(
                new ProbabilityFactor(probability, causeId)))
            .WithNodeValue(value)
            .WithId(id)
            .Build();
    }

    private static Fact<TFactValue> CreateFactWithOperation<TFactValue>(
        TFactValue value, ProbabilityFactor[] edges,
        Func<ProbabilityFactor[], CausesOperation> operation)
    {
        return new FactBuilder<TFactValue>()
            .WithNodeValue(value)
            .WithCausesExpression(operation(edges))
            .Build();
    }

    public static Fact<TFactValue> CreateFactWithAnd<TFactValue>(TFactValue value,
        params ProbabilityFactor[] edges)
    {
        return CreateFactWithOperation(value, edges, Expressions.And);
    }

    public static Fact<TFactValue> CreateFactWithOr<TFactValue>(TFactValue value,
        params ProbabilityFactor[] edges)
    {
        return CreateFactWithOperation(value, edges, Expressions.Or);
    }

    public static Fact<TFactValue> CreateVariant<TFactValue>(
        string abstractNodeId, float weight, TFactValue? value = default)
    {
        return new FactBuilder<TFactValue>()
            .WithAbstractFactId(abstractNodeId)
            .WithWeights(new List<WeightFactor> {
                new WeightFactor(weight)
            })
            .WithNodeValue(value)
            .Build();
    }

    public static List<Fact<TFactValue>> CreateAbstractFactVariants<TFactValue>(
        Fact<TFactValue> abstractFact, params TFactValue[] variants)
    {
        var res = new List<Fact<TFactValue>>() { };

        foreach (var val in variants)
        {
            var node = CreateVariant(abstractFact.Id, 1, val);
            res.Add(node);
        }

        return res;
    }
}
