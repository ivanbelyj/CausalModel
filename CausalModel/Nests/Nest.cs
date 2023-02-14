using CausalModel.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Nests
{
    public abstract class Nest
    {
        /// <summary>
        /// Определяет, является ли гнездо причин корневым. Все ребра, входящие
        /// в такие гнезда, не ссылаются на узлы модели.
        /// </summary>
        public bool IsRootNest()
        {
            foreach (var edge in GetEdges())
            {
                if (edge.CauseId != null)
                    return false;
            }
            return true;
        }

        public abstract IEnumerable<CausalEdge> GetEdges();
    }
}
