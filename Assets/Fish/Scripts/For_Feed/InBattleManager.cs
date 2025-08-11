using UnityEngine;

public class InBattleManager : MonoBehaviour
{

    // バトル中であるフラグを餌ごとに管理する
    // 各餌に対して、バトル中の魚がいる状態では他のバトルを始めないようにする

    public bool isInBattle { get; private set; } = false; // バトル中であるフラグ

    public bool GetInBattle() => isInBattle; // ゲッター
    public void SetTrueInBattle() => isInBattle = true; // trueのセッター
    public void SetFalseInBattle() => isInBattle = false; // falseのセッター

}