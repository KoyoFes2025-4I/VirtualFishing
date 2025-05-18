using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThingsToFish))]
// [RequireComponent(typeof(FishBattle))]

public class BattleManager : MonoBehaviour
{
    // �R���g���[���[�ǂɃo�g�������̃v���O�����͍���Ă��炤
    // ThingsToFish�R���|�[�l���g���Q�Ƃ����āA�v���C���[�̓��͂Ƌ��I�u�W�F�N�g�̃p�����[�^�Ńo�g������i�v�����j
    // ���̒��ŃZ�b�^�[���g����wasSuccessFishing�i�o�g���̏��s�j��wasFinishFishing�i�o�g���I���j�̃t���O�͊Ǘ����Ă��炤�z��

    private ThingsToFish fish;
    // private FishBattle fishBattle;�i���j

    // �ˑ��R���|�[�l���g�̏���������
    protected virtual void Awake()
    {
        fish = GetComponent<ThingsToFish>();
        // fishBattle = GetComponent<FishBattle>();
    }

    // �R���[�`�����g���ăA�j���[�V�����Đ��ƃo�g���������񓯊��Ɏ��s�����悤�݌v����
    public void DoBattle()
    {
        StartCoroutine(BattleRoutine()); // �R���[�`���̊J�n
    }

    private IEnumerator BattleRoutine()
    {
    
        fish.SetTrueInBattle(); // �o�g�����ł���t���O��true�ɂ���i�A�j���[�V�����̍Đ������t���[���Ăяo�����j

        yield return StartCoroutine(WaitForBattleResult()); // �����őҋ@

        fish.SetFalseInBattle(); // �o�g�����ł���t���O��false�ɂ���i�A�j���[�V�����̍Đ��͏I���j

    }

    // �o�g���̏I����҂���
    private IEnumerator WaitForBattleResult()
    {

        // �����̃I�u�W�F�N�g��HP�Ɨ͂̃p�����[�^���擾
        int fishStrength = fish.Strength;
        int fishPower = fish.Power;

        // FishBattle���\�b�h���Ă�Ńo�g���������J�n
        // fishBattle.FishBattle(fishStrength, fishPower);

        // ������wasFinishFishing�̃t���O��true�ɂȂ��Ă���
        // ����Ƀo�g���ɏ����Ă�����wasSuccessFishing��true�ɂȂ��Ă���i��������false�̂܂܁j

        // ���̏��������ƃR���g���[���[�ǂ̃v���O�����̕��ɂ��񓯊��������K�v�ɂȂ�H

        yield return new WaitUntil(() => fish.wasFinishFishing); // �o�g�����I�������R���[�`���I��

    }

}