using UnityEngine;
using UnityEngine.Rendering;

public class FishBattle : MonoBehaviour
{

    // �o�g������

    private ThingsToFish fish;

    public void Battle(int FishStrength, int FIshPower)
    {
        fish.SetSuccessFishing(); // �o�g���ɏ���
        fish.SetFinishFishing(); // �o�g���I��
    }

}
