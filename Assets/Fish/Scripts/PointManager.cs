using UnityEngine;

public class PointManager : MonoBehaviour
{
    private int SumPoint = 0;

    // �Q�b�^�[
    public int GetSumPoint() => SumPoint;

    // �l�������|�C���g��SumPoint�֑������\�b�h
    public void AddPoint(int point)
    {
        SumPoint += point;
    }

}