using UnityEngine;

public class InBattleManager : MonoBehaviour
{

    // �o�g�����ł���t���O���a���ƂɊǗ�����
    // �e�a�ɑ΂��āA�o�g�����̋��������Ԃł͑��̃o�g�����n�߂Ȃ��悤�ɂ���

    public bool isInBattle { get; private set; } = false; // �o�g�����ł���t���O

    public bool GetInBattle() => isInBattle; // �Q�b�^�[
    public void SetTrueInBattle() => isInBattle = true; // true�̃Z�b�^�[
    public void SetFalseInBattle() => isInBattle = false; // false�̃Z�b�^�[

}