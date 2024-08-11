using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks
{
    public class DeclaredBlock
    {
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        public string Convention { get; set; }

        /// <summary>
        /// Null for declared block injected externally (abstract block)
        /// </summary>
        public string? CausesConvention { get; set; }

        /// <summary>
        /// External Fact Ids By Block CauseIds
        /// </summary>
        public Dictionary<string, string> CauseBlockReferencesMap { get; set; }
            = new Dictionary<string, string>();

        public DeclaredBlock(
            string id,
            string convention,
            string? causesConvention,
            Dictionary<string, string>? causeBlockReferencesMap)
        {
            Id = id;
            Convention = convention;
            CausesConvention = causesConvention;

            if (causeBlockReferencesMap != null)
            {
                CauseBlockReferencesMap = causeBlockReferencesMap;
            }
        }

        /// <summary>
        /// Constructor used for deserialization. Should be used for deserialization only
        /// </summary>
        private DeclaredBlock() : this(null!, null!, null, null) { }

        /// <summary>
        /// Block cause id to external model fact id
        /// </summary>
        public string GetActualExternalFactId(string blockCauseId)
        {
            return CauseBlockReferencesMap[blockCauseId];
        }

        public string? TryGetActualExternalFactId(string blockCauseId)
        {
            var res = CauseBlockReferencesMap.TryGetValue(blockCauseId, out var actualFactId);
            return res ? actualFactId : null;
        }
    }
}
