using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CausalModel.Serialization.Mapping
{
    internal static class MappingUtils
    {
        public static Common.CausalBundle<TFactValue> Map<TFactValue>(
            CausalBundle<TFactValue> bundle)
            where TFactValue : class
        {
            return new Common.CausalBundle<TFactValue>()
            {
                BlockCausesConventions = bundle.BlockCausesConventions.Select(x => Map(x)).ToList(),
                BlockConventions = bundle.BlockConventions.Select(x => Map(x)).ToList(),
                BlockResolvingMap = bundle.BlockResolvingMap ?? new Blocks.Resolving.BlockResolvingMap(),
                CausalModels = bundle.CausalModels.Select(x => Map(x)).ToList(),
                DefaultMainModel = bundle.DefaultMainModel,
            };
        }

        public static CausalBundle<TFactValue> Map<TFactValue>(
            Common.CausalBundle<TFactValue> bundle)
            where TFactValue : class
        {
            return new CausalBundle<TFactValue>()
            {
                BlockCausesConventions = bundle.BlockCausesConventions.Select(x => Map(x)).ToList(),
                BlockConventions = bundle.BlockConventions.Select(x => Map(x)).ToList(),
                BlockResolvingMap = bundle.BlockResolvingMap ?? new Blocks.Resolving.BlockResolvingMap(),
                CausalModels = bundle.CausalModels.Select(x => Map(x)).ToList(),
                DefaultMainModel = bundle.DefaultMainModel,
            };
        }

        private static Model.CausalModel<TFactValue> Map<TFactValue>(
            CausalModel<TFactValue> model)
            where TFactValue : class
        {
            return new Model.CausalModel<TFactValue>(model.Name)
            {
                Name = model.Name,
                Facts = model.Facts,
                DeclaredBlocks = model.DeclaredBlocks.Select(x => Map(x)).ToList(),
            };
        }

        private static Blocks.DeclaredBlock Map(DeclaredBlock block)
        {
            return new Blocks.DeclaredBlock(
                block.Id,
                block.Convention,
                block.CausesConvention,
                block.BlockCausesMap,
                block.BlockConsequencesMap);
        }

        private static CausalModel<TFactValue> Map<TFactValue>(
            Model.CausalModel<TFactValue> model)
            where TFactValue : class
        {
            return new CausalModel<TFactValue>()
            {
                Name = model.Name,
                Facts = model.Facts,
                DeclaredBlocks = model.DeclaredBlocks.Select(x => Map(x)).ToList(),
            };
        }

        private static DeclaredBlock Map(Blocks.DeclaredBlock block)
        {
            return new DeclaredBlock(block.Id)
            {
                BlockCausesMap = block.BlockCausesMap,
                BlockConsequencesMap = block.BlockConsequencesMap.ToDictionary(x => x.Key, x => x.Value),
                CausesConvention = block.CausesConvention,
                Convention = block.Convention,
            };
        }

        private static BlockConvention Map(Blocks.BlockConvention blockConvention)
        {
            return new BlockConvention(blockConvention.Name)
            {
                Consequences = blockConvention.Consequences
            };
        }

        private static Blocks.BlockConvention Map(BlockConvention blockConvention)
        {
            return new Blocks.BlockConvention(blockConvention.Name)
            {
                Consequences = blockConvention.Consequences,
            };
        }

        private static Blocks.BlockCausesConvention Map(
            BlockCausesConvention blockCausesConvention)
        {
            return new Blocks.BlockCausesConvention(blockCausesConvention.Name)
            {
                Causes = blockCausesConvention.Causes,
            };
        }

        private static BlockCausesConvention Map(
            Blocks.BlockCausesConvention blockCausesConvention)
        {
            return new BlockCausesConvention(blockCausesConvention.Name)
            {
                Causes = blockCausesConvention.Causes,
            };
        }
    }
}
