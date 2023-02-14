using CausalModel.Edges;
using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Nests
{
    public class WeightNest : Nest
    {
        private List<WeightEdge> edges { get; } = new List<WeightEdge>();
        public List<WeightEdge> Weights => edges;

        public WeightNest(params WeightEdge[] edges)
        {
            this.edges = edges.ToList();
        }

        /// <summary>
        /// Создает весовое гнездо, которое имеет единственное весовое ребро
        /// </summary>
        public WeightNest(Guid? causeId, double weight = 1)
        {
            edges.Add(new WeightEdge(weight, causeId));
        }

        /// <summary>
        /// Подсчитывает общий вес гнезда, основываясь на весовых ребрах, связанных
        /// с произошедшими причинными событиями
        /// </summary>
        /// <returns></returns>
        public double TotalWeight()
        {
            if (edges.Count == 0)
                throw new InvalidOperationException("Весовое гнездо не имеет ребер");

            double weightSum = 0;

            foreach (var edge in edges)
            {
                var cause = (IHappenable?)edge.Cause;

                // Если у весового гнезда нет причины, считается, что вес всегда влияет на выбор
                if (cause is null)
                {
                    weightSum += edge.Weight;
                    continue;
                }

                //if (cause.IsHappened is null)
                //    throw new NullReferenceException("На этапе вычисления суммы весового гнезда "
                //        + " не было определено, произошло ли причинное событие");

                if (cause.IsHappened)
                    weightSum += edge.Weight;
            }

            return weightSum;
        }

        public override IEnumerable<CausalEdge> GetEdges() => edges;
    }
}
