using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Instance;
public class InstanceFactAddress
{
    public string FactId { get; }

    /// <summary>
    /// Id of the model instance where the fact is used as external or actually located
    /// </summary>
    public string ModelInstanceId { get; }
    public InstanceFactAddress(string factId, string causalInstanceId)
    {
        FactId = factId;
        ModelInstanceId = causalInstanceId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FactId, ModelInstanceId);
    }
}
