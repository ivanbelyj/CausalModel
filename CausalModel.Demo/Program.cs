using CausalModel.Demo;
using CausalModel.Model;

// Todo: ?
// Если CauseId для WeightEdge не указан, выбор реализации абстрактного факта
// работает некорректно
// 

// Todo: вернуть сборку CausalModel.Tests

string fileName = UserInteraction.GetFileName();
CausalModelManager manager = new CausalModelManager();
manager.Init(fileName);
manager.RunModel();
