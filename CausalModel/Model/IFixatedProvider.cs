using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model
{
    /// <summary>
    /// Предоставляет информацию о том, зафиксировано ли происшествие
    /// факта с заданным Id
    /// </summary>
    public interface IFixatedProvider
    {
        public bool? IsFixated(Guid factId);
    }
}
