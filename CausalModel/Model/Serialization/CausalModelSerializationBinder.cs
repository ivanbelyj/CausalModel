using CausalModel.CausesExpressionTree;
using CausalModel.Factors;
using CausalModel.Facts;
using Newtonsoft.Json.Serialization;

namespace CausalModel.Model.Serialization;
public class CausalModelSerializationBinder<TFactValue> : ISerializationBinder
{
    // Polymorphic serialization/deserialization is required for
    // CausesExpression => And, Or, FactorLeaf, Not
    // (Nests - no)
    // (Factors - no)
    // Fact => FactVariant

    public static readonly List<(Type type, string name)> KnownTypes
        = new List<(Type, string)>() {
        (typeof(ConjunctionOperation), "and"),
        (typeof(DisjunctionOperation), "or"),
        (typeof(FactorLeaf), "factor"), 
        (typeof(InversionOperation), "not"),
    };
    //public static List<(Type type, string name)> KnownTypes => knownTypes;

    public void BindToName(Type serializedType, out string? assemblyName,
        out string? typeName)
    {
        typeName = KnownTypes.FirstOrDefault(x => x.type == serializedType).name;
        if (typeName == null)
        {
            throw new ArgumentException("Not registred type for binding: "
                + serializedType.Name);
        }
        assemblyName = null;
    }

    public Type BindToType(string? assemblyName, string typeName)
    {
        Type? type = KnownTypes.FirstOrDefault(x => x.name == typeName).type;
        if (type == null)
        {
            throw new ArgumentException("Incorrect type name for binding: "
                + typeName);
        }
        return type;
    }
}
