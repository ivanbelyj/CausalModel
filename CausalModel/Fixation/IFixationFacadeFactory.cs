using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation
{
    public interface IFixationFacadeFactory<TFactValue>
        where TFactValue : class
    {
        FixationFacade<TFactValue> Create();
    }
}
