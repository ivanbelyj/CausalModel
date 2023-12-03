using CausalModel.Factors;
using CausalModel.Model;
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
        private List<WeightFactor> edges { get; } = new List<WeightFactor>();
        public List<WeightFactor> Weights => edges;

        public WeightNest(params WeightFactor[] edges)
        {
            this.edges = edges.ToList();
        }

        /// <summary>
        /// Создает весовое гнездо, которое имеет единственное весовое ребро
        /// </summary>
        public WeightNest(Guid? causeId, double weight = 1)
        {
            edges.Add(new WeightFactor(weight, causeId));
        }

        /// <summary>
        /// Подсчитывает общий вес гнезда, основываясь на весовых ребрах, связанных
        /// с произошедшими причинными событиями
        /// </summary>
        /// <returns></returns>
        public double TotalWeight(IFixatedProvider happenedProvider)
        {
            if (edges.Count == 0)
                throw new InvalidOperationException("Весовое гнездо не имеет ребер");

            double weightSum = 0;

            foreach (var edge in edges)
            {
                // Если у весового гнезда нет причины, считается, что вес всегда влияет на выбор
                if (edge.CauseId == null)
                {
                    weightSum += edge.Weight;
                    continue;
                } else
                {
                    bool? isHappened = happenedProvider.IsFixated(edge.CauseId.Value);
                    if (isHappened != null && isHappened.Value)
                    {
                        weightSum += edge.Weight;
                    }
                }
                    
            }

            return weightSum;
        }

        public override IEnumerable<CausalEdge> GetEdges() => edges;
    }
}
