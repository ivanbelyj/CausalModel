using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks.Resolving
{
    /// <summary>
    /// Maps implementing models with conventions or block names for resolving purposes.
    /// </summary>
    public class BlockResolvingMap
    {
        public Dictionary<string, string> ModelNamesByConventionName
        {
            get; set;
        } = new Dictionary<string, string>();

        public Dictionary<string, string> ModelNamesByDeclaredBlockId
        {
            get; set;
        } = new Dictionary<string, string>();
    }
}