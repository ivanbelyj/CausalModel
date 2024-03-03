using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Facts
{
    /// <summary>
    /// The most abstract fact class agnostic about its causes.
    /// All that we can say about fact via BaseFact is whether it fixated or not
    /// </summary>
    public class BaseFact
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
