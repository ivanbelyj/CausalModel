using CausalModel.Demo;
using CausalModel.Model;

// Todo: ?
// Если CauseId для WeightEdge не указан, выбор реализации абстрактного факта
// работает некорректно
// 

string fileName = UserInteraction.GetFileName();
CausalModelManager manager = new();
manager.Init(fileName);
manager.RunModel();
