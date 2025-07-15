using UnityEngine;

public class Fish1 : ThingsToFish
{

    // フィールドの値はinspectorから設定
    // 親クラスで定義したOnCollisionEnterやAwakeなどは自動で実行される

    protected override void WinFishing()
    {
        base.WinFishing();

        // オーバーライド
    }

    protected override void LoseFishing()
    {
        base.LoseFishing();

        // オーバーライド
    }

}