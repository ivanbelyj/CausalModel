using CausalModel.Demo;
using CausalModel.Demo.Utils;
using CausalModel.Model;

ConfigureAnsiCodes.Configure();

string fileName = FileUtils.GetFileName();
DemoRunner demoRunner = new(fileName);
demoRunner.Run();
