using UnityEngine;

public class PointManager : MonoBehaviour
{

    [SerializeField] private int PlayerID; // プレイヤーID
    private int SumPoint = 0;

    // ゲッター
    public int GetSumPoint() => SumPoint;
    public int GetPlayerID() => PlayerID;

    // 獲得したポイントをSumPointへ加算するメソッド
    public void AddPoint(int point)
    {
        SumPoint += point;
    }

}