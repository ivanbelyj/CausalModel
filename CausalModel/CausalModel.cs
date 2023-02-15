using CausalModel.Edges;
using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel
{
    public class CausalModel<TNodeValue>
    {
        public event Action<Fact<TNodeValue>>? FactHappened;

        private CausalData<TNodeValue> causalData;
        private Dictionary<Guid, bool> factsHappened = new Dictionary<Guid, bool>();

        private Dictionary<Fact<TNodeValue>,
            List<Fact<TNodeValue>>> causesAndConsequences;
        private Dictionary<Fact<TNodeValue>,
            List<FactVariant<TNodeValue>>> factsAndVariants;

        public CausalModel(CausalData<TNodeValue> causalData) {
            CausalData = causalData;
        }

        public CausalData<TNodeValue> CausalData
        {
            get => causalData;
            private set
            {
                causalData = value;
                Initialize();
            }
        }

        public void Fixate(Guid factId, bool? factHappened = null)
        {
            Fact<TNodeValue> fact = causalData.GetFactById(factId);
            // true для выполнения условия следования факта из причин, необходимого,
            // но не всегда достаточного для происшествия
            bool followsFromCauses;

            // Если происшествие факта задается явно (например, событие уже
            // произошло в игре)
            if (factHappened != null)
            {
                followsFromCauses = factHappened.Value;
            } else  // Иначе происшествие определяется на основе причин
            {
                bool? isHappened = fact.ProbabilityNest.CausesExpression.Evaluate();
                // Случай, когда недостаточно данных (некоторые причины
                // еще не зафиксированы)
                if (isHappened == null)
                {
                    return;
                }
                followsFromCauses = isHappened.Value;
            }

            // Для происшествия обычных фактов достаточно условия следования из причин
            if (!(fact is FactVariant<TNodeValue>))
            {
                factsHappened[fact.Id] = followsFromCauses;

                FixateNotFixedConsequences(fact);
            } else {
                // Вариант факта происходит только после того, как он будет
                // выбран в качестве единственной реализации абстрактного факта.

                var variant = (FactVariant<TNodeValue>)fact;
                var abstractFact = causalData.GetFactById(variant.AbstractFactId);

                var selectedVariant = SelectFactVariant(factsAndVariants[abstractFact]);
                foreach (var factVariant in factsAndVariants[abstractFact])
                {
                    factsHappened[factVariant.Id] = ReferenceEquals(factVariant,
                        selectedVariant);
                    FixateNotFixedConsequences(factVariant);
                }
            }
        }

        private void FixateNotFixedConsequences(Fact<TNodeValue> fact)
        {
            foreach (var consequence in causesAndConsequences[fact])
            {
                if (!factsHappened.ContainsKey(consequence.Id))
                    Fixate(consequence.Id);
            }
        }

        private void Initialize()
        {
            causesAndConsequences = new Dictionary<Fact<TNodeValue>,
                    List<Fact<TNodeValue>>>();
            foreach (Fact<TNodeValue> fact in causalData.Nodes)
            {
                foreach (var edge in fact.GetEdges())
                {
                    if (edge.CauseId.HasValue)
                    {
                        var cause = causalData.GetFactById(edge.CauseId.Value);
                        if (!causesAndConsequences.ContainsKey(cause))
                        {
                            causesAndConsequences.Add(cause,
                                new List<Fact<TNodeValue>>() { fact });
                        }
                        else
                        {
                            causesAndConsequences[cause].Add(fact);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Случайно выбирает одну из реализаций факта, учитывая веса вариантов
        /// </summary>
        private FactVariant<TNodeValue>? SelectFactVariant(
            List<FactVariant<TNodeValue>> variants)
        {
            Random rnd = new Random();

            // Собрать информацию о узлах и их общих весах, собрать сумму весов,
            // а также отбросить узлы с нулевыми весами
            var nodesWeights = new List<(Fact<TNodeValue> node, double totalWeight)>();
            double weightsSum = 0;
            foreach (var node in variants)
            {
                double totalWeight = node.WeightNest.TotalWeight();
                if (totalWeight >= double.Epsilon)
                {
                    nodesWeights.Add((node, totalWeight));
                    weightsSum += totalWeight;
                }
            }
            if (weightsSum < double.Epsilon)
                return null;

            // Сумма вероятностей для выбора единственной реализации
            // double weightsSum = nodes.Sum(node => node.WeightNest.TotalWeight());

            // Определить Id единственной реализации
            // Алгоритм Roulette wheel selection
            double choice = rnd.NextDouble(0, weightsSum);
            int curNodeIndex = -1;
            while (choice >= 0)
            {
                curNodeIndex++;
                if (curNodeIndex >= variants.Count)
                    curNodeIndex = 0;

                // choice -= nodes[curNodeIndex].WeightNest.TotalWeight();
                choice -= nodesWeights[curNodeIndex].totalWeight;
            }
            return variants[curNodeIndex];
        }
    }
}
