using UnityEngine;
using UnityEngine.Rendering;

public class FishBattle : MonoBehaviour
{

    // �o�g������

    private ThingsToFish fish;

    void Start()
    {
        fish = GetComponent<ThingsToFish>();
    }

    public void Battle(int FishStrength, int FIshPower)
    {
        fish.SetSuccessFishing(); // �o�g���ɏ����i�ڐG���ăo�g���ɓ������玩���I�ɏ������o�j
        fish.SetFinishFishing(); // �o�g���I��
    }

}
