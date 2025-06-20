using UnityEngine;

[RequireComponent(typeof(ThingsToFish))]
// [RequireComponent(typeof(FishBattle))]

public class BattleManager : MonoBehaviour
{

    private ThingsToFish fish;
    private FishBattle fishBattle;

    // 依存コンポーネントの初期化処理
    protected virtual void Awake()
    {
        fish = GetComponent<ThingsToFish>();
        fishBattle = GetComponent<FishBattle>();
    }

    public void DoBattle()
    {

        fish.SetTrueInBattle(); // バトル中であるフラグをtrueにする

        // 魚側のオブジェクトのHPと力のパラメータを取得
        int fishStrength = fish.Strength;
        int fishPower = fish.Power;

        // Battleメソッドを呼んでバトル処理を開始
        fishBattle.Battle(fishStrength, fishPower);

        // ここでwasFinishFishingのフラグはtrueになっている
        // さらにバトルに勝っていたらwasSuccessFishingもtrueになっている（負けたらfalseのまま）

        fish.SetFalseInBattle(); // バトル中であるフラグをfalseにする（アニメーションの再生は終了）

    }

}