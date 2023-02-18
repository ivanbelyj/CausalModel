﻿using CausalModel.Factors;
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
            => And(edges.Select(edge => new EdgeLeaf(edge)).ToArray());

        public static DisjunctionOperation Or(params CausesExpression[] expressions)
            => new DisjunctionOperation(expressions);
        public static DisjunctionOperation Or(params ProbabilityFactor[] edges)
            => Or(edges.Select(edge => new EdgeLeaf(edge)).ToArray());

        public static InversionOperation Not(CausesExpression expr)
            => new InversionOperation(expr);
        public static InversionOperation Not(ProbabilityFactor edge)
            => new InversionOperation(new EdgeLeaf(edge));

        public static EdgeLeaf Edge(ProbabilityFactor edge)
            => new EdgeLeaf(edge);
        public static EdgeLeaf Edge(float probability, Guid? causeId = null)
            => new EdgeLeaf(new ProbabilityFactor(probability, causeId));
    }
}
