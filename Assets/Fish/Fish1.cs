using UnityEngine;

public class Fish1 : ThingsToFish
{

    // オブジェクトのフィールドはinspectorから設定する
    // 何も書かなくても親クラスで定義したOnCollisionEnter、Awake、Updateなどはそのまま実行される

    protected override void MovementConfig()
    {

        base.MovementConfig(); // 親クラスに書かれている処理

        // 何かこのオブジェクトで固有の処理を書きたい場合はオーバーライドできる

    }

    protected override void WinFishing()
    {

        base.WinFishing();

        // オーバーライド可能

    }

    protected override void LoseFishing()
    {

        base.LoseFishing();

        // オーバーライド可能

    }

}