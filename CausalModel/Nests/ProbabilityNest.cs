﻿//using CausalModel.CausesExpressionTree;
//using CausalModel.Factors;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CausalModel.Nests
//{
//    /// <summary>
//    /// Предоставляет и структурирует совокупность причинных ребер, определяющую
//    /// зависимость следствия от причины
//    /// </summary>
//    public class ProbabilityNest : Nest
//    {
//        /// <summary>
//        /// Причинные ребра, структурированные в виде логического выражения. Значение
//        /// выражения после вычисления определяет, произошло ли событие (узел модели)
//        /// </summary>
//        public CausesExpression CausesExpression { get; set; }

//        // For deserialization
//        public ProbabilityNest() : this(null) { }
//        public ProbabilityNest(CausesExpression expression)
//        {
//            CausesExpression = expression;
//        }

//        public ProbabilityNest(float probability, Guid? causeId)
//        {

//            ProbabilityFactor edge = new ProbabilityFactor(probability, causeId);
//            CausesExpression = new FactorLeaf(edge);
//        }

//        /// <summary>
//        /// Определяет, имеет ли место событие или факт, представленный узлом каузальной
//        /// модели, в конкретной генерируемой ситуации, на основе логического выражения,
//        /// структурирующего причинные связи.
//        /// Допустимо вызывать, только если известно, имеют ли место все причины события.
//        /// </summary>
//        // public bool IsHappened() => CausesExpression.Evaluate();

//        public override IEnumerable<ProbabilityFactor> GetEdges()
//            => CausesExpression.GetEdges();
//    }
//}
