using CausalModel.Demo;
using CausalModel.Model;

// Todo: ?
// ���� CauseId ��� WeightEdge �� ������, ����� ���������� ������������ �����
// �������� �����������
// 

string fileName = UserInteraction.GetFileName();
CausalModelManager manager = new CausalModelManager();
manager.Init(fileName);
manager.RunModel();
