using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Factors
{
    /// <summary>
    /// Представляет причинно-следственное ребро, которое указывает, с какой вероятностью
    /// один из факторов следствия происходит в случае происшествия причины.
    /// </summary>
    public class ProbabilityFactor : Factor
    {
        private float probability;
        /// <summary>
        /// Вероятность того, что причинно-следственная связь повлечет за собой
        /// фактор события. Значение от 0 (фактор не происходит никогда) до 1.0
        /// (фактор происходит в любом случае)
        /// включительно <br />
        /// </summary>
        public float Probability
        {
            get => probability;
            set
            {
                if (value >= 0 && value <= 1.0)
                {
                    probability = value;
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(Probability),
                        "Incorrect probability value. It must be in [0, 1]");
            }
        }

        public ProbabilityFactor(float probability, string? causeId = null)
        {
            Probability = probability;
            CauseId = causeId;
        }

        public override string ToString()
        {
            string str = $"Probability: {Probability}; ";
            str += $"CauseId: {CauseId}";
            return str;
        }

        /// <summary>
        /// Функция, определяющая, повлекла ли причинно-следственная связь за собой
        /// фактор события в текущей генерации при данных вероятности и фиксурующем
        /// значении. Значение вероятности превалирует над фиксирующим значением
        /// (если вероятность определена как 0 или 1, любое фиксирующее значение не может
        /// повлиять на исход фактора)
        /// </summary>
        public static bool IsHappened(float probability, float fixingValue)
            => probability - fixingValue > 0;
    }
}
