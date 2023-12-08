using CausalModel.Factors;
using CausalModel.Nests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts;
public class BlockFact<TNodeValue> : Fact<TNodeValue>
{
    public IEnumerable<Factor> Causes { get; set; } = new List<Factor>();
    public override IEnumerable<Factor> GetCauses()
    {
        return Causes;
    }
}
