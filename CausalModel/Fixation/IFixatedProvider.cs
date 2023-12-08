using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation
{
    /// <summary>
    /// Provides information about whether the fact with a given id has been fixated
    /// </summary>
    public interface IFixatedProvider
    {
        public bool? IsFixated(string factId);
    }
}
