using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Factors
{
    public class Factor
    {
        /// <summary>
        /// Id of the fact that is the cause. Null for the root facts
        /// that have no causes from the point of view of the model
        /// </summary>
        public string? CauseId { get; set; }
    }
}
