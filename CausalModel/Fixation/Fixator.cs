using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation
{
    public class Fixator<TFactValue> : IFixator<TFactValue>
    {
        private Dictionary<InstanceFactId, bool> factIdsFixated =
            new Dictionary<InstanceFactId, bool>();

        public event FactFixatedEventHandler<TFactValue>? FactFixated;

        public bool? IsFixated(InstanceFactId id)
        {
            bool isInDict = factIdsFixated.TryGetValue(id, out var isHappened);
            if (isInDict)
                return isHappened;
            else
                return null;
        }

        public virtual void FixateFact(InstanceFactId factId, bool isHappened)
        {
            factIdsFixated[factId] = isHappened;
            FactFixated?.Invoke(this, factId, isHappened);
        }

        public void Reset()
        {
            factIdsFixated = new Dictionary<InstanceFactId, bool>();
        }
    }
}
