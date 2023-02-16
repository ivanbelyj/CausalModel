using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel
{
    /// <summary>
    /// Предоставляет информацию о том, произошел ли факт с заданным Id
    /// </summary>
    public interface IHappenedProvider
    {
        public bool IsHappened(Guid factId);
    }
}
