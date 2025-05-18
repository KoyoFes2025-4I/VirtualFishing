using UnityEngine;

public class FishAnimations : MonoBehaviour
{

    // �e�A�j���[�V������inspector����ݒ肵�Ă���
    [SerializeField] private Animator moveAnimator;
    [SerializeField] private Animator caughtAnimator;
    [SerializeField] private Animator winAnimator;
    [SerializeField] private Animator loseAnimator;

    // �X�e�[�g���̐ݒ�
    private string moveAnimationName = "Swim";
    private string caughtAnimationName = "Caught";
    private string winAnimationName = "Win";
    private string loseAnimationName = "Lose";

    //�e�A�j���[�V�������Đ����郁�\�b�h��p��
    public void PlayMoveAnimation()
    {
        moveAnimator?.Play(moveAnimationName);
    }

    public void PlayCaughtAnimation()
    {
        caughtAnimator?.Play(caughtAnimationName);
    }

    public void PlayWinAnimation()
    {
        winAnimator?.Play(winAnimationName);
    }

    public void PlayLoseAnimation()
    {
        loseAnimator?.Play(loseAnimationName);
    }

}
