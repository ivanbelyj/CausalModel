using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model
{
    public interface IFixingValueProvider
    {
        public float GetFixingValue();
    }
}
