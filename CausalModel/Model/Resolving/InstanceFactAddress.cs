using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{

    /// <summary>
    /// Allows to get an instance fact from the resolved model provider
    /// both by its actual location and by the location where it's used
    /// (in some models as an external cause or as a block consequence).
    /// An instance fact can be addressed by several addresses
    /// </summary>
    public class InstanceFactAddress
    {
        /// <summary>
        /// Local fact id in the model
        /// </summary>
        public string FactId { get; }

        /// <summary>
        /// Id of the model instance in which the fact is used or actually located
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

        public override string ToString()
        {
            return $"InstanceId: {ModelInstanceId}, FactId: {FactId}";
        }
    }
}
