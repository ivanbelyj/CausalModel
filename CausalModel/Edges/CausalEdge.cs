using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Edges
{
    public class CausalEdge
    {
        /// <summary>
        /// Guid узла, представляющего причину. null для корневых узлов
        /// </summary>
        public Guid? CauseId { get; set; }
    }
}
