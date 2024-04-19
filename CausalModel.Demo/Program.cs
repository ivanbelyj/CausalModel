using CausalModel.Demo;
using CausalModel.Demo.Utils;
using CausalModel.Model;

string fileName = FileUtils.GetFileName();
DemoRunner demoRunner = new(fileName);
demoRunner.Run();
