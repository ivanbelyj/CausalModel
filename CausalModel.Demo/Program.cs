using CausalModel.Demo;
using CausalModel.Model;

// Todo: ?
// ���� CauseId ��� WeightEdge �� ������, ����� ���������� ������������ �����
// �������� �����������
// 

string fileName = UserInteraction.GetFileName();
CausalModelManager manager = new();
manager.Init(fileName);
manager.RunModel();
