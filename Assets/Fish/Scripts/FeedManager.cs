using UnityEngine;

[RequireComponent(typeof(InWaterManager))]

public class FeedManager : MonoBehaviour
{

    private InWaterManager inWaterManager;

    [SerializeField] private GameObject model; // 餌の3Dモデル（Blenderで作成）

    private Vector3 FeedPosition; // 餌オブジェクトの位置

    // 着水時にInWaterManagerから「釣り竿をどれだけ強く振ったか」のような数値を返してもらう想定
    // その情報から餌の位置FeedPosition（3次元ベクトル）を計算して設定する

    private void Start()
    {
        inWaterManager = GetComponent<InWaterManager>();
    }

    private void Update()
    {
       　transform.position = FeedPosition; // オブジェクトの座標を変更
    }

    // 餌の座標のゲッター
    public Vector3 GetFeedPosition() => FeedPosition;

}
