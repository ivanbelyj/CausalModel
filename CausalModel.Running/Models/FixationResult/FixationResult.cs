using CausalModel.Blocks;
using CausalModel.Model.Instance;
using CausalModel.Running.FixationRunning;
using CausalModel.Running.Models.Common;
using CausalModel.Running.Models.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.Models.FixationResult
{
    public class FixationResult<TFactValue>
        where TFactValue : class
    {
        public int Seed { get; set; }
        public long ElapsedMilliseconds { get; set; }

        public Dictionary<string, FixationResultModelElement<FactInfo<TFactValue>, TFactValue>> ResultModelElementsByModelName { get; set; }
            = new Dictionary<string, FixationResultModelElement<FactInfo<TFactValue>, TFactValue>>();
        public Dictionary<string, FixationResultModelInstanceElement<TFactValue>> ModelInstanceElementsByModelInstanceId { get; set; } = new Dictionary<string, FixationResultModelInstanceElement<TFactValue>>();

        public override string ToString()
        {
            var converter = new FixationResultToStringConverter<TFactValue>(this, new ConverterOptions());
            return converter.Convert();
        }
    }
}
