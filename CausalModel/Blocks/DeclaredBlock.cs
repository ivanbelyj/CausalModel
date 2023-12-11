using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks;
public class DeclaredBlock
{
    [JsonProperty(Required = Required.Always)]
    public string Id { get; set; }

    // Null - "any"
    public string? Convention { get; set; }


    // Should be used for deserialization.
    private DeclaredBlock() : this(null!, null)
    {
    }

    public DeclaredBlock(string id, string? convention)
    {
        Id = id;
        Convention = convention;
    }
}
