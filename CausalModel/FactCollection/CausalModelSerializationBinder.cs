using CausalModel.CausesExpressionTree;
using CausalModel.Model;
using CausalModel.Nodes;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.FactCollection
{
    public class CausalModelSerializationBinder<TNodeValue> : ISerializationBinder
    {
        private static readonly List<(Type type, string name)> knownTypes
            = new List<(Type, string)>() {
            (typeof(ConjunctionOperation), "and"),
            (typeof(DisjunctionOperation), "or"),
            (typeof(FactorLeaf), "factor"),
            (typeof(InversionOperation), "not"),

            (typeof(Fact<TNodeValue>), "fact"),
            (typeof(FactVariant<TNodeValue>), "variant"),

            //(typeof(CausalModel<TNodeValue>), "causal-model"),
            //(typeof(FactCollection<TNodeValue>), "fact-collection"),
        };
        public static List<(Type type, string name)> KnownTypes => knownTypes;

        public void BindToName(Type serializedType, out string? assemblyName,
            out string? typeName)
        {
            typeName = knownTypes.FirstOrDefault(x => x.type == serializedType).name;
            if (typeName == null)
            {
                throw new ArgumentException("Not registred type for binding: "
                    + serializedType.FullName);
            }
            assemblyName = null;
        }

        public Type BindToType(string? assemblyName, string typeName)
        {
            Type? type = knownTypes.FirstOrDefault(x => x.name == typeName).type;
            if (type == null)
            {
                throw new ArgumentException("Incorrect type name for binding: "
                    + typeName);
            }
            return type;
        }
    }
}
