using UnityEngine;

public class Fish1 : ThingsToFish
{

    // �I�u�W�F�N�g�̃t�B�[���h��inspector����ݒ肷��
    // ���������Ȃ��Ă��e�N���X�Œ�`����OnCollisionEnter�AAwake�AUpdate�Ȃǂ͂��̂܂܎��s�����

    protected override void MovementConfig()
    {

        base.MovementConfig(); // �e�N���X�ɏ�����Ă��鏈��

        // �������̃I�u�W�F�N�g�ŌŗL�̏��������������ꍇ�̓I�[�o�[���C�h�ł���

    }

    protected override void WinFishing()
    {

        base.WinFishing();

        // �I�[�o�[���C�h�\

    }

    protected override void LoseFishing()
    {

        base.LoseFishing();

        // �I�[�o�[���C�h�\

    }

}