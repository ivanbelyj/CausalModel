using CausalModel.Common.DataProviders;
using CausalModel.Model;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks.Resolving
{
    public class BlockResolver<TFactValue> : IBlockResolver<TFactValue>
        where TFactValue : class
    {
        private readonly ModelInstanceFactory<TFactValue> modelInstanceFactory;
        private readonly IConventionsProvider conventionsProvider;
        private readonly IModelsProvider<TFactValue> modelsProvider;
        private readonly IBlockImplementationSelector blockImplementationSelector;

        public event BlockImplementedEventHandler<TFactValue>? BlockImplemented;

        public BlockResolver(
            ModelInstanceFactory<TFactValue> modelInstanceFactory,
            IConventionsProvider conventionsProvider,
            IModelsProvider<TFactValue> modelsProvider,
            IBlockImplementationSelector blockImplementationSelector)
        {
            this.modelInstanceFactory = modelInstanceFactory;
            this.conventionsProvider = conventionsProvider;
            this.modelsProvider = modelsProvider;
            this.blockImplementationSelector = blockImplementationSelector;
        }

        public ModelInstance<TFactValue> Resolve(
            DeclaredBlock block,
            ModelInstance<TFactValue> parentInstance)
        {
            var convention = GetBlockConvention(block, parentInstance);
            var causesConvention = GetBlockCausesConvention(block, parentInstance);

            var model = GetBlockImplementation(block);

            if (convention != null)
            {
                EnsureConventionsImplemented(causesConvention, convention, model, parentInstance);
            }

            var instance = modelInstanceFactory.InstantiateModel(model);

            BlockImplemented?.Invoke(this, block, convention, instance);
            return instance;
        }

        private BlockConvention? GetBlockConvention(
            DeclaredBlock block,
            ModelInstance<TFactValue> parentInstance)
        {
            string? conv = block.Convention;
            return conv == null ? null : conventionsProvider.GetConventionByName(conv);
        }

        private BlockCausesConvention? GetBlockCausesConvention(
            DeclaredBlock block,
            ModelInstance<TFactValue> parentInstance)
        {
            string? conv = block.CausesConvention;
            return conv == null ? null : conventionsProvider.GetCauseConventionByName(conv);
        }

        private CausalModel<TFactValue> GetBlockImplementation(DeclaredBlock block)
        {
            string implementationName = blockImplementationSelector
                .GetImplementationModelName(block);

            return modelsProvider.GetModelByName(implementationName);
        }

        // Todo: move into validator?
        private void EnsureConventionsImplemented(
            BlockCausesConvention? causesConvention,
            BlockConvention convention,
            CausalModel<TFactValue> externalModel,
            ModelInstance<TFactValue> parent)
        {
            // TODO:

            //var notImplementedConsequences = GetNotImplementedConsequences(
            //    convention,
            //    externalModel,
            //    parent);
            //var notImplementedCauses = causesConvention == null
            //    ? new List<Factor>()
            //    : GetNotImplementedCauses(causesConvention, externalModel, parent);

            //if (notImplementedConsequences.Any() || notImplementedCauses.Any())
            //{
            //    throw new BlockResolvingException(
            //        $"Failed to resolve block convention "
            //        + $"(name: {convention.Name}): model provided by resolver "
            //        + "is not matching.")
            //    {
            //        NotImplementedCauses = notImplementedCauses,
            //        NotImplementedConsequences = notImplementedConsequences
            //    };
            //}
        }

        //private List<string> GetNotImplementedCauses(
        //    DeclaredBlock declaredBlock,
        //    BlockCausesConvention causesConvention,
        //    CausalModel<TFactValue> externalModel,
        //    ModelInstance<TFactValue> parent)
        //{
        //    List<string>? notImplementedCauses = new List<string>();

        //    if (causesConvention.Causes != null)
        //    {
        //        foreach (var cause in causesConvention.Causes)
        //        {
        //            var externalFactId = declaredBlock.GetExternalFactId(cause);
        //            var fact = externalModel.Facts.Find(fact => fact.Id == externalFactId);
        //            if (fact == null)
        //            {
        //                notImplementedCauses.Add(cause);
        //            }
        //        }
        //    }

        //    return notImplementedCauses;
        //}

        //private List<string> GetNotImplementedConsequences(
        //    DeclaredBlock declaredBlock,
        //    BlockConvention convention,
        //    CausalModel<TFactValue> externalModel,
        //    ModelInstance<TFactValue> parent)
        //{
        //    List<string>? notImplementedConsequences = new List<string>();

        //    if (convention.Consequences != null)
        //    {
        //        foreach (var consequence in convention.Consequences)
        //        {
        //            // TODO:

        //            var fact = externalModel.Facts.Find(fact => fact.Id == externalFactId);
        //            if (fact == null)
        //            {
        //                notImplementedConsequences.Add(consequence);
        //            }
        //        }
        //    }

        //    return notImplementedConsequences;
        //}
    }
}
