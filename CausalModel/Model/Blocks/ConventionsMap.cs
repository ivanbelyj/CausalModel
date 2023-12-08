using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Blocks;
public class ConventionsMap<TFactValue>
{
    public Dictionary<string, CausalModel<TFactValue>> ModelsByConventionName
    {
        get; set;
    } = new();
}
