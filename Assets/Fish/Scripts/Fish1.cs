using UnityEngine;

public class Fish1 : ThingsToFish
{

    // フィールドはinspectorから設定
    // 親クラスで定義したOnCollisionEnter、Awake、Updateなどは自動で実行される

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

    protected override void Update()
    {
        base.Update();

        // オーバーライド
    }

}