using UnityEngine;
using UnityEngine.InputSystem;

public class InWaterManager : MonoBehaviour
{
    // 着水フラグの管理

    public bool isInWater { get; private set; } = false; // 釣り竿の着水フラグ

    void Update()
    {
        // テスト用で、スペースキーを押すたびに着水判定を変える（Gameビューでないと反応しない）
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isInWater = !isInWater;

            if (isInWater)
            {
                Debug.Log("着水判定 : true");
            }
            else
            {
                Debug.Log("着水判定 : false");
            }

        }
    }

    public bool GetInWater() => isInWater; // ゲッター
    public void SetTrueInWater() => isInWater = true; // trueのセッター
    public void SetFalseInWater() => isInWater = false; // falseのセッター

}