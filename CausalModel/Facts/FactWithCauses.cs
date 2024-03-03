using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts
{
    public abstract class FactWithCauses : BaseFact
    {
        public abstract IEnumerable<Factor> GetCauses();
        public bool IsRootCause()
        {
            return GetCauses().All(x => x.CauseId == null);
        }
    }
}
