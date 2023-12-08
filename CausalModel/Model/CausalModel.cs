using CausalModel.Blocks;
using CausalModel.Model.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;
public class CausalModel<TNodeValue>
{
    public FactCollection<TNodeValue>? Facts { get; set; }
    public IEnumerable<BlockConvention>? BlockConventions { get; set; }
    public IEnumerable<DeclaredBlock>? Blocks { get; set; }
}
