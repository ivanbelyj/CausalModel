using CausalModel.Demo;
using CausalModel.Model;

// Todo: ?
// ���� CauseId ��� WeightEdge �� ������, ����� ���������� ������������ �����
// �������� �����������
// 

// Todo: ������� ������ CausalModel.Tests

string fileName = UserInteraction.GetFileName();
CausalModelManager manager = new CausalModelManager();
manager.Init(fileName);
manager.RunModel();
