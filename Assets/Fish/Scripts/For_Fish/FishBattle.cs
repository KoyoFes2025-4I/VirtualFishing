using UnityEngine;
using UnityEngine.Rendering;

public class FishBattle : MonoBehaviour
{

    // 実際のバトル処理関数

    private ThingsToFish fish;

    void Start()
    {
        fish = GetComponent<ThingsToFish>();
    }

    public void Battle(int FishStrength, int FishPower)
    {
        // 50%ずつで勝敗が決まる設定
        if (Random.Range(-1f, 1f) >= 0.5f)
        {
            fish.SetSuccessFishing(); // 勝利フラグを立てておく
            fish.SetFinishFishing(); // バトル終了（勝利時）
        }

        else
        {
            fish.SetFinishFishing(); // バトル終了（敗北時）
        }
            
    }

}
