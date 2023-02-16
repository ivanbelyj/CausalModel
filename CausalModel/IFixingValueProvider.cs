using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel
{
    public interface IFixingValueProvider
    {
        public float GetFixingValue();
    }
}
