using UnityEngine;

public class PointManager : MonoBehaviour
{

    [SerializeField] private int PlayerID; // �v���C���[ID
    private int SumPoint = 0;

    // �Q�b�^�[
    public int GetSumPoint() => SumPoint;
    public int GetPlayerID() => PlayerID;

    // �l�������|�C���g��SumPoint�։��Z���郁�\�b�h
    public void AddPoint(int point)
    {
        SumPoint += point;
    }

}