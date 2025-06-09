using UnityEngine;

public class Fish1 : ThingsToFish
{

    // フィールドはinspectorから設定
    // 親クラスで定義したOnCollisionEnter、Awake、Updateなどは自動で実行される

    protected override void MovementConfig()
    {

        base.MovementConfig(); // 親クラスに書かれている処理

        // 何かこのオブジェクトで固有の処理を書きたい場合はオーバーライド

    }

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