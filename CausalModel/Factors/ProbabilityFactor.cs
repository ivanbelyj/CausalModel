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
    public class ProbabilityFactor : CausalEdge
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
                    throw new ArgumentOutOfRangeException("",
                        "Некорректное значение вероятности");
            }
        }

        //private double? fixingValue;
        /// <summary>
        /// Значение, фиксирующее вероятность и определяющее, повлекла ли
        /// причинно-следственная связь за собой фактор события в текущей генерации.
        /// Значение больше или равно 0 (фактор происходит при любой ненулевой вероятности)
        /// и строго меньше 1.0
        /// Может принимать значение null, например, до генерации
        /// </summary>
        //public double? FixingValue
        //{
        //    get => fixingValue;
        //    set
        //    {
        //        if (value is null || value >= 0 && value < 1.0)
        //        {
        //            fixingValue = value;
        //        }
        //        else
        //            throw new ArgumentOutOfRangeException("",
        //                "Некорректное фиксирующее значение");
        //    }
        //}

        public ProbabilityFactor(float probability, Guid? causeId = null)
        {
            Probability = probability;
            CauseId = causeId;
            //FixingValue = actualProbability;
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
