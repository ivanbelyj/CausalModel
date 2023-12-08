using CausalModel.Factors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts;
public abstract class AbstractFact
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public abstract IEnumerable<Factor> GetCauses();
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public bool IsRootCause()
    {
        return GetCauses().All(x => x.CauseId == null);
    }
}
