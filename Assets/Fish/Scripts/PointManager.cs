using UnityEngine;

public class PointManager : MonoBehaviour
{
    private int SumPoint = 0;

    // ゲッター
    public int GetSumPoint() => SumPoint;

    // 獲得したポイントをSumPointへ足すメソッド
    public void AddPoint(int point)
    {
        SumPoint += point;
    }

}