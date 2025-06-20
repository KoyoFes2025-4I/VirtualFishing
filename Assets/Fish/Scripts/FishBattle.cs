using UnityEngine;
using UnityEngine.Rendering;

public class FishBattle : MonoBehaviour
{

    // バトル処理

    private ThingsToFish fish;

    public void Battle(int FishStrength, int FIshPower)
    {
        fish.SetSuccessFishing(); // バトルに勝利
        fish.SetFinishFishing(); // バトル終了
    }

}
