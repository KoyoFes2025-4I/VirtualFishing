using UnityEngine;
using UnityEngine.Rendering;

public class FishBattle : MonoBehaviour
{

    // 実際のバトル処理関数

    private ThingsToFish fish;

    private int strength;
    private int power;
    private bool isBattling = false;

    public void Battle(int FishStrength, int FishPower)
    {
        strength = FishStrength;
        power = FishPower;
        isBattling = true; // Battle関数が呼び出された時下記のUpdate関数をアクティブ化
    }

    void Start()
    {
        fish = GetComponent<ThingsToFish>();
    }

    private void Update()
    {
        if (!isBattling) return; // isBattlingがfalseだと何もしない

        // テスト用にWキーを押したら勝利
        if (Input.GetKeyDown(KeyCode.W))
        {
            fish.SetSuccessFishing(); // 勝利フラグを立てる
            fish.SetFinishFishing(); // バトル終了（勝利）
            isBattling = false;
        }

        // テスト用にLキーを押したら敗北
        else if (Input.GetKeyDown(KeyCode.L))
        {
            fish.SetFinishFishing(); // バトル終了（敗北）
            isBattling = false;
        }
    }
}