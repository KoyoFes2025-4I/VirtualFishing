using UnityEngine;

public class InWaterManager : MonoBehaviour
{

    // ��������

    private ThingsToFish fish;

    void Update()
    {
        // �X�y�[�X�L�[���������璅��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fish.SetIsInWater();
        }
    }

}
