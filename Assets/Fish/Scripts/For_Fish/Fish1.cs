using UnityEngine;

public class Fish1 : ThingsToFish
{

    // フィールドの値はinspectorから設定
    // 親クラスで定義したOnCollisionEnterやAwakeなどは自動で実行される

    public override void Lose()
    {
        base.Lose();

        // オーバーライド
    }

    public override void Win()
    {
        base.Win();

        // オーバーライド
    }

}