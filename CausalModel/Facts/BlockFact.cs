using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts;
public class BlockFact : FactWithCauses
{
    public string ConventionName { get; set; }
    public IEnumerable<Factor> Causes { get; set; } = new List<Factor>();
    public IEnumerable<BaseFact> Consequences { get; set; }
        = new List<BaseFact>();

    public BlockFact(string conventionName)
    {
        ConventionName = conventionName;
    }

    public override IEnumerable<Factor> GetCauses()
    {
        //var baseCauses = base.GetCauses();
        //baseCauses.AddRange(Causes);
        //return baseCauses;
        return Causes;
    }
}
