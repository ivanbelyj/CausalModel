using CausalModel.Blocks;
using CausalModel.Model.Instance;
using CausalModel.Running.Extensions;
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
        public long ElapsedMilliseconds => Elapsed.Milliseconds;
        public long ElapsedNanoseconds => Elapsed.GetTotalNanoseconds();
        public TimeSpan Elapsed { get; set; }

        public Dictionary<string, FixationResultModelInfo<FactInfo<TFactValue>, TFactValue>> ModelInfoByName { get; set; }
            = new Dictionary<string, FixationResultModelInfo<FactInfo<TFactValue>, TFactValue>>();
        public Dictionary<string, FixationResultModelInstanceInfo<TFactValue>> ModelInstanceInfoById { get; set; }
            = new Dictionary<string, FixationResultModelInstanceInfo<TFactValue>>();

        public FixationResult(TimeSpan elapsed)
        {
            Elapsed = elapsed;
        }

        public override string ToString()
        {
            var converter = new FixationResultToStringConverter<TFactValue>(this, new ConverterOptions());
            return converter.Convert();
        }
    }
}
