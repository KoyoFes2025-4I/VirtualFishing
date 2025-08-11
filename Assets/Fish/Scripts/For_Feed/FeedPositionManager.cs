using UnityEngine;

public class FeedPositionManager : MonoBehaviour
{

    [SerializeField] private Vector3 FeedPosition; // 餌オブジェクトの位置

    // 着水時にコントローラーから「釣り竿をどれだけ強く振ったか」のような数値を返してもらう
    // その情報から餌の位置のFeedPosition（3次元ベクトル）を計算して更新させるようにする

    private void Update()
    {
       transform.position = FeedPosition; // オブジェクトの座標を変更
    }

    // 餌の座標のゲッター
    public Vector3 GetFeedPosition() => FeedPosition;

}
