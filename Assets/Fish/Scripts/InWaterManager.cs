using UnityEngine;

public class InWaterManager : MonoBehaviour
{

    // ��������

    private ThingsToFish fish;

    void Start()
    {
        fish = GetComponent<ThingsToFish>();
    }

    void Update()
    {
        // �X�y�[�X�L�[���������璅��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fish.SetIsInWater();
            Debug.Log("�������܂����B");
        }
    }

}
