
using CausalModel.Experiments;
using CausalModel.Model;
using CausalModel.Model.Serialization;

const string modelFilePath = "models/races2.json";
string modelJson = File.ReadAllText(modelFilePath);
CausalModel<string>? model = CausalModelSerialization.FromJson<string>(modelJson);
if (model == null)
    throw new Exception("No model deserialized");

var helper = new ExperimentsHelper(model);
helper.Run();
