using CausalModel.Common.Extensions;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Fixation.Random
{
    internal class RandomProvider : IRandomProvider
    {
        private readonly System.Random sharedRandom;

        private readonly int baseSeed;

        private System.Random dynamicRandom;

        public int BaseSeed => baseSeed;

        public RandomProvider(int? baseSeed = null)
        {
            this.baseSeed = baseSeed ?? new System.Random().Next();
            dynamicRandom = new System.Random(this.baseSeed * 11);
            sharedRandom = new System.Random(unchecked(this.baseSeed * 17));
        }

        // Mixing base seed with additional data is supposed to ensure lower sensitivity
        // of the generation results to the order in which the facts were fixated
        public void SetDynamicSeed(string modelInstanceId, string factId)
        {
            // Ignored for now because it slows down the fixation
            // Todo: implement efficient random not sensitive to fixation order.
            // Sensitiveness can be issue if facts are fixated manually (for example, in real-time)

            //unchecked
            //{
            //    int hash1 = modelInstanceId?.GetHashCode() ?? 0;
            //    int hash2 = factId?.GetHashCode() ?? 0;

            //    long combinedHash = ((long)hash1 << 32) | (hash2 & 0xFFFFFFFFL);
            //    dynamicRandom = new System.Random((int)(combinedHash ^ baseSeed));
            //}
        }

        public float NextFloatSharedRandom(float min, float max)
        {
            return NextFloatCore(sharedRandom, min, max);
        }

        float IRandomProvider.NextFloat(float min, float max)
        {
            return NextFloatCore(dynamicRandom, min, max);
        }

        private float NextFloatCore(System.Random random, float min, float max)
        {
            return (float)random.NextDouble(min, max);
        }
    }
}
