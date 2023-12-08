using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.CausesExpressionTree
{
    public static class Expressions
    {
        public static ConjunctionOperation And(params CausesExpression[] expressions)
            => new ConjunctionOperation(expressions);
        public static ConjunctionOperation And(params ProbabilityFactor[] edges)
            => And(edges.Select(edge => new FactorLeaf(edge)).ToArray());

        public static DisjunctionOperation Or(params CausesExpression[] expressions)
            => new DisjunctionOperation(expressions);
        public static DisjunctionOperation Or(params ProbabilityFactor[] edges)
            => Or(edges.Select(edge => new FactorLeaf(edge)).ToArray());

        public static InversionOperation Not(CausesExpression expr)
            => new InversionOperation(expr);
        public static InversionOperation Not(ProbabilityFactor edge)
            => new InversionOperation(new FactorLeaf(edge));

        public static FactorLeaf Edge(ProbabilityFactor edge)
            => new FactorLeaf(edge);
        public static FactorLeaf Edge(float probability, string? causeId = null)
            => new FactorLeaf(new ProbabilityFactor(probability, causeId));
    }
}
