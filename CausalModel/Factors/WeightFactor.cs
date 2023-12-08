using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Factors
{
    /// <summary>
    /// Весовое ребро делает определенный вариант реализации абстрактной сущности (АС)
    /// более или менее благоприятным для выбора
    /// </summary>
    public class WeightFactor : Factor
    {
        /// <summary>
        /// Вес, определяющий благоприятность определенного варианта реализации <br/>
        /// абстрактной сущности для выбора. <br/>
        /// Значение больше или равно 0, устанавливается перед генерацией относительно <br />
        /// весов других реализаций. При 0 весовое ребро не учитывается, при отрицательных
        /// значениях поведение не определено
        /// </summary>
        public double Weight { get; set; }

        public WeightFactor(double weight, string? causeId = null)
        {
            Weight = weight;
            CauseId = causeId;
        }
    }
}
