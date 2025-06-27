using UnityEngine;
using UnityEngine.Rendering;

public class FishBattle : MonoBehaviour
{

    // バトル処理

    private ThingsToFish fish;

    void Start()
    {
        fish = GetComponent<ThingsToFish>();
    }

    public void Battle(int FishStrength, int FIshPower)
    {
        fish.SetSuccessFishing(); // バトルに勝利（接触してバトルに入ったら自動的に勝利演出）
        fish.SetFinishFishing(); // バトル終了
    }

}
