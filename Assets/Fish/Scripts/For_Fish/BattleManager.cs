using UnityEngine;

[RequireComponent(typeof(ThingsToFish))]
[RequireComponent(typeof(FishBattle))]

public class BattleManager : MonoBehaviour
{

    private ThingsToFish fish;
    private FishBattle fishBattle;

    // �ˑ��R���|�[�l���g�̏���������
    protected virtual void Awake()
    {
        fish = GetComponent<ThingsToFish>();
        fishBattle = GetComponent<FishBattle>();
    }

    public void DoBattle()
    {

        // �����̃I�u�W�F�N�g��HP�Ɨ͂̃p�����[�^���擾
        int fishStrength = fish.Strength;
        int fishPower = fish.Power;

        // Battle���\�b�h���Ă�Ńo�g���������J�n
        fishBattle.Battle(fishStrength, fishPower);

        // ������wasFinishFishing�̃t���O��true�ɂȂ��Ă���i�o�g���I���j
        // ����Ƀo�g���ɏ����Ă�����wasSuccessFishing��true�ɂȂ��Ă���i��������false�̂܂܁j

    }

}