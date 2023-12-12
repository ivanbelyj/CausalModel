using CausalModel.Factors;
using CausalModel.Facts;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks.Resolving;
public abstract class BlockResolverBase<TFactValue> : IBlockResolver<TFactValue>
{
    public event BlockImplementedEventHandler<TFactValue>? BlockImplemented;

    protected virtual void CheckConvention(BlockConvention convention,
        CausalModel<TFactValue> model, ModelInstance<TFactValue> parent)
    {
        List<Factor>? notImplementedCauses = new();
        List<BaseFact>? notImplementedConsequences = new();

        if (convention.Consequences != null)
        {
            foreach (var consequence in convention.Consequences)
            {
                var fact = model.Facts.Find(fact => fact.Id == consequence.Id);
                if (fact == null)
                {
                    notImplementedConsequences.Add(consequence);
                }
            }
        }

        if (convention.Causes != null)
        {
            foreach (var cause in convention.Causes)
            {
                var fact = parent.Model.Facts.Find(fact => fact.Id == cause.CauseId);
                if (fact == null)
                {
                    notImplementedCauses.Add(cause);
                }
            }
        }

        if (notImplementedConsequences.Any()
            || notImplementedCauses.Any()) {
            throw new BlockResolvingException($"Failed to resolve block convention "
                + $"(name: {convention.Name}): model provided by resolver "
                + "is not matching.")
            {
                NotImplementedCauses = notImplementedCauses,
                NotImplementedConsequences = notImplementedConsequences
            };
        }
    }

    public abstract CausalModel<TFactValue> GetConventionImplementation(
        DeclaredBlock block,
        BlockConvention? convention);

    public ModelInstance<TFactValue> Resolve(DeclaredBlock block,
        ModelInstance<TFactValue> parentInstance)
    {
        string? convName = block.Convention;
        BlockConvention? convention = convName == null ? null
            : parentInstance.Model.GetConventionByName(convName);

        var model = GetConventionImplementation(block, convention);

        if (convention != null)
            CheckConvention(convention, model, parentInstance);

        var instance = new ModelInstance<TFactValue>(model);

        BlockImplemented?.Invoke(this, block, convention, instance);
        return instance;
    }
}
