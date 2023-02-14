using CausalModel.Edges;
using CausalModel.Nests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Nodes
{
    /// <summary>
    /// В моделируемой ситуации факт может быть представлен в виде одного из своих вариантов.
    /// Например, при генерации персонажа факт исповедания религии может быть представлен
    /// одним из вариантов факта, представляющих конкретные верования.
    /// Варианты факта могут не только отличаться значениями узла, но и местом в
    /// каузальной модели.
    /// </summary>
    public class FactVariant<TNodeValue> : Fact<TNodeValue>
    {
        public WeightNest WeightNest { get; set; }
        public Guid AbstractFactId { get; set; }

        // Для десериализации
        public FactVariant() : base()
        {
            WeightNest = new WeightNest();
        }

        public FactVariant(Guid id, Guid abstractNodeId, TNodeValue? value,
            WeightNest weightNest, ProbabilityNest? probabilityNest = null)
            : base(id, probabilityNest ?? new ProbabilityNest(null, 1), value)
        {
            AbstractFactId = abstractNodeId;
            WeightNest = weightNest;
        }

        public FactVariant(Guid abstractNodeId, WeightNest weightNest,
            TNodeValue? value, ProbabilityNest probabilityNest)
            : this(Guid.NewGuid(), abstractNodeId, value, weightNest, probabilityNest)
        { }

        public override IEnumerable<CausalEdge> GetEdges()
            => WeightNest.GetEdges().Concat(
                (IEnumerable<CausalEdge>)ProbabilityNest.GetEdges());
        public override bool IsRootNode()
            => WeightNest.IsRootNest() && ProbabilityNest.IsRootNest();
    }
}
