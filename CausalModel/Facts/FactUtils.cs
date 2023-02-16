using CausalModel.CausesExpressionTree;
using CausalModel.Edges;
using CausalModel.Nests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Nodes
{
    public static class FactUtils
    {
        public static Fact<TNodeValue> CreateNode<TNodeValue>(
            float probability, TNodeValue? value = default, Guid? causeId = null)
            => new Fact<TNodeValue>(new ProbabilityNest(probability, causeId), value);

        private static Fact<TNodeValue> CreateNodeWithOperation<TNodeValue>(
            TNodeValue value, ProbabilityEdge[] edges,
            Func<ProbabilityEdge[], CausesOperation> operation)
            => new Fact<TNodeValue>(new ProbabilityNest(operation(edges)), value);

        /// <summary>
        /// Создает узел, имеющий множество причинных ребер. Ребра объединены
        /// логической операцией И
        /// </summary>
        public static Fact<TNodeValue> CreateNodeWithAnd<TNodeValue>(TNodeValue value,
            params ProbabilityEdge[] edges)
            => CreateNodeWithOperation(value, edges, Expressions.And);

        /// <summary>
        /// Создает узел, имеющий множество причинных ребер. Ребра объединены
        /// логической операцией ИЛИ
        /// </summary>
        public static Fact<TNodeValue> CreateNodeWithOr<TNodeValue>(TNodeValue value,
            params ProbabilityEdge[] edges)
            => CreateNodeWithOperation(value, edges, Expressions.Or);

        /// <summary>
        /// Создает узел-реализацию, связанный с абстрактным узлом единственным весовым ребром.
        /// Также узел имеет вероятностное ребро, обеспечивающее безусловную причину существования
        /// данного варианта реализации
        /// </summary>
        /// <returns></returns>
        public static FactVariant<TNodeValue> CreateVariant<TNodeValue>(
            Guid abstractNodeId, float weight, TNodeValue? value = default)
            => new FactVariant<TNodeValue>(abstractNodeId,
                new WeightNest(abstractNodeId, weight),
                new ProbabilityNest(1, null), value);

        public static List<Fact<TNodeValue>> CreateAbstractFact<TNodeValue>(
            Fact<TNodeValue> abstractFact, params TNodeValue[] variants)
        {
            var res = new List<Fact<TNodeValue>>() { abstractFact };

            foreach (var val in variants)
            {
                var node = FactUtils.CreateVariant(abstractFact.Id, 1, val);
                res.Add(node);
            }
            return res;
        }
    }
}
