using UnityEngine;

public class Fish1 : ThingsToFish
{

    // �t�B�[���h�̒l��inspector����ݒ�
    // �e�N���X�Œ�`����OnCollisionEnter��Awake�Ȃǂ͎����Ŏ��s�����

    protected override void WinFishing()
    {
        base.WinFishing();

        // �I�[�o�[���C�h
    }

    protected override void LoseFishing()
    {
        base.LoseFishing();

        // �I�[�o�[���C�h
    }

}