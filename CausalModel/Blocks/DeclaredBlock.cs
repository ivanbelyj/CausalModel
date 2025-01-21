using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks
{
    /// <summary>
    /// A block, declared to be used in a specific causal model. It has consequences
    /// that can be used by <b>external</b> model, and causes,
    /// that <b>parent</b> model must provide. Causes are external facts for the block,
    /// consequences are external facts for the model, that uses the block.
    /// Ids of external facts are mapped, so every causal model can use several blocks
    /// satisfying the same convention without conflicts of ids.
    /// </summary>
    public class DeclaredBlock
    {
        public string Id { get; set; }

        /// <summary>
        /// Name of the block convention defining consequences that can be used
        /// in the external model
        /// </summary>
        public string Convention { get; set; }

        /// <summary>
        /// Name of the block causes convention defining the facts that the parent model
        /// must provide for the block.
        /// </summary>
        public string CausesConvention { get; set; }

        /// <summary>
        /// Provides external fact ids by local cause id in block.
        /// </summary>
        public Dictionary<string, string> BlockCausesMap { get; set; }
            = new Dictionary<string, string>();

        /// <summary>
        /// Provides external consequence ids by local fact id in block.
        /// Null is correct for default convention only.
        /// <para>
        /// <b>Getting / setting of this property is not optimized and supposed
        /// to be used for not very often operations like serialization.
        /// Please, use <see cref="MappedExternalToLocal"/> for converting ids</b>
        /// </para>
        /// </summary>
        public IReadOnlyDictionary<string, string> BlockConsequencesMap
        {
            get
            {
                return localIdsByExternalConsequenceId.ToDictionary(x => x.Value, x => x.Key);
            }
            set
            {
                localIdsByExternalConsequenceId = value.ToDictionary(x => x.Value, x => x.Key);
            }
        }

        /// <summary>
        /// Provides local cause ids in block by external fact ids.
        /// </summary>
        private Dictionary<string, string>? localIdsByExternalConsequenceId
            = new Dictionary<string, string>();

        public DeclaredBlock(
            string id,
            string convention,
            string causesConvention,
            Dictionary<string, string> blockCausesMap,
            Dictionary<string, string> blockConsequencesMap)
        {
            Id = id;
            Convention = convention;
            CausesConvention = causesConvention;

            if (blockCausesMap != null)
            {
                BlockCausesMap = blockCausesMap;
            }
            if (blockConsequencesMap != null)
            {
                BlockConsequencesMap = blockConsequencesMap;
            }
        }

        public IEnumerable<string> GetMappedExternalCauses()
        {
            return BlockCausesMap.Values;
        }

        public string? LocalCauseIdToMappedExternalFactId(string blockCauseId)
        {
            return TryGetFromDictionary(BlockCausesMap, blockCauseId);
        }

        public string? MappedExternalToLocal(string externalCausalId)
        {
            return TryGetFromDictionary(localIdsByExternalConsequenceId, externalCausalId);
        }

        private string? TryGetFromDictionary(
            Dictionary<string, string>? dictionary,
            string key)
        {
            if (dictionary == null)
            {
                return null;
            }

            var containsKey = dictionary.TryGetValue(key, out var value);
            return containsKey ? value : null;
        }
    }
}
