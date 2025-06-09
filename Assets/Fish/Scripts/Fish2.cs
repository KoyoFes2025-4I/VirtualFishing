using UnityEngine;

public class Fish2 : ThingsToFish
{

    protected override void MovementConfig()
    {

        base.MovementConfig(); // 親クラスに書かれている処理

        // オーバーライド

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