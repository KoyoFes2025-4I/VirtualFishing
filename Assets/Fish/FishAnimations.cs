using UnityEngine;

public class FishAnimations : MonoBehaviour
{

    // 各アニメーションはinspectorから設定しておく
    [SerializeField] private Animator moveAnimator;
    [SerializeField] private Animator caughtAnimator;
    [SerializeField] private Animator winAnimator;
    [SerializeField] private Animator loseAnimator;

    // ステート名の設定
    private string moveAnimationName = "Swim";
    private string caughtAnimationName = "Caught";
    private string winAnimationName = "Win";
    private string loseAnimationName = "Lose";

    //各アニメーションを再生するメソッドを用意
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
