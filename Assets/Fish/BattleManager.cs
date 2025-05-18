using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThingsToFish))]
// [RequireComponent(typeof(FishBattle))]

public class BattleManager : MonoBehaviour
{
    // コントローラー班にバトル処理のプログラムは作ってもらう
    // ThingsToFishコンポーネントを参照させて、プレイヤーの入力と魚オブジェクトのパラメータでバトルする（要検討）
    // その中でセッターを使ってwasSuccessFishing（バトルの勝敗）とwasFinishFishing（バトル終了）のフラグは管理してもらう想定

    private ThingsToFish fish;
    // private FishBattle fishBattle;（仮）

    // 依存コンポーネントの初期化処理
    protected virtual void Awake()
    {
        fish = GetComponent<ThingsToFish>();
        // fishBattle = GetComponent<FishBattle>();
    }

    // コルーチンを使ってアニメーション再生とバトル処理が非同期に実行されるよう設計する
    public void DoBattle()
    {
        StartCoroutine(BattleRoutine()); // コルーチンの開始
    }

    private IEnumerator BattleRoutine()
    {
    
        fish.SetTrueInBattle(); // バトル中であるフラグをtrueにする（アニメーションの再生が毎フレーム呼び出される）

        yield return StartCoroutine(WaitForBattleResult()); // ここで待機

        fish.SetFalseInBattle(); // バトル中であるフラグをfalseにする（アニメーションの再生は終了）

    }

    // バトルの終了を待つ処理
    private IEnumerator WaitForBattleResult()
    {

        // 魚側のオブジェクトのHPと力のパラメータを取得
        int fishStrength = fish.Strength;
        int fishPower = fish.Power;

        // FishBattleメソッドを呼んでバトル処理を開始
        // fishBattle.FishBattle(fishStrength, fishPower);

        // ここでwasFinishFishingのフラグはtrueになっている
        // さらにバトルに勝っていたらwasSuccessFishingもtrueになっている（負けたらfalseのまま）

        // この書き方だとコントローラー班のプログラムの方にも非同期処理が必要になる？

        yield return new WaitUntil(() => fish.wasFinishFishing); // バトルが終わったらコルーチン終了

    }

}