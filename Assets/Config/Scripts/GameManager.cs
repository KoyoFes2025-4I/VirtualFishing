using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] ThingGenerator thingGenerator; // 魚オブジェクトの管理クラス
    [SerializeField] RodsController rodsController; // 釣り竿の管理クラス
    [SerializeField] Config config; // コンフィグUI画面の管理クラス
    [SerializeField] NetworkManager networkManager; // FlaskのAPIサーバー接続用クラス

    // Userクラスのインスタンスが要素となるリストを用意
    public List<User> waitUsers { get; } = new List<User>(); // 待機中ユーザーリスト
    public List<User> nextUsers { get; } = new List<User>(); // 次のゲームに参加予定のユーザーリスト
    public List<User> gamingUsers { get; } = new List<User>(); // 現在ゲーム中のユーザーリスト

    public bool isGaming { get; private set; } = false; // ゲーム進行中フラグ

    // 「準備」→「スタート」→「終了」→「準備」...で進行する

    // ゲーム開始処理（スタートボタン押下時）
    public void StartGame()
    {
        if (isGaming) return; // ゲーム進行中は開始処理を実行できない
        isGaming = true;

        // 参加予定ユーザーリストをリセット
        nextUsers.Clear();

        // 待機ユーザーリストから竿の本数だけ参加予定プレイヤーに割り当てる
        for (int i = 0; i < rodsController.RodsCount && waitUsers.Count > 0; i++)
        {
            nextUsers.Add(waitUsers[0]);
            waitUsers.RemoveAt(0);
        }

        // nextUsersはリセットされてwaitUsersから竿の本数だけユーザーが追加される
        // waitUsersは先頭から竿の数だけnextUsersに渡されることで減る

        // オブジェクト生成を有効化してゲームをスタートさせる
        thingGenerator.isGenerate = true;
        rodsController.ShowMessage("スタート！", 3);

        // UIのListViewを更新させる
        config.ListViewRefresh();
    }

    // ゲーム準備処理（準備ボタン押下時）
    public void Prepare()
    {
        if (isGaming) return; // ゲーム進行中は準備処理を実行できない

        // ゲーム参加者を更新する
        // gamingUsersを一度リセットして新しくnextUsersを全てコピー
        gamingUsers.Clear();
        gamingUsers.AddRange(nextUsers);

        // オブジェクト生成を無効化して全てのオブジェクトを破棄する
        thingGenerator.isGenerate = false;
        thingGenerator.Regenerate();

        // 釣り竿にユーザーを対応させる
        rodsController.SetUsers(gamingUsers);
        rodsController.UpdateRods();

        // UIのListViewを更新させる
        config.ListViewRefresh();
    }

    // 次のゲームに参加するユーザーを一度リセットしてからセットするメソッド
    public void SetNextUsers(List<string> users)
    {
        nextUsers.Clear();
        foreach (string userName in users) nextUsers.Add(new User(userName));
    }

    // 待機中ユーザーを一度リセットしてからセットするメソッド
    public void SetWaitUsers(List<string> users)
    {
        waitUsers.Clear();
        foreach (string userName in users) waitUsers.Add(new User(userName));
    }

    // ゲーム終了処理（終了ボタン押下時）
    public void FinishGame()
    {
        isGaming = false; // ゲーム中フラグはfalseにする

        // 遊び終わったユーザー（gamingUsers）の各データコンテナを全てデータベースへ保存する
        // この時点でユーザー名・得点・釣った魚のリストは全てデータとしてUserクラスに入っている
        foreach (User user in gamingUsers)
        {
            user.NetworkManager = networkManager;
            user.Save();
        }

        // 竿の状態をリセット
        rodsController.Reset();
        rodsController.ShowResult();

        // オブジェクト生成を無効化して全てのオブジェクトを破棄する
        thingGenerator.isGenerate = false;
        thingGenerator.Regenerate();
    }
}

// プレイヤー情報についてのデータコンテナ
[Serializable]
public class User
{
    public NetworkManager NetworkManager { get; set; } // Flask接続用

    public string name = ""; // ユーザー名
    public int point = 0; // 得点
    public List<string> fishedThingNames = new List<string>(); // 釣り上げた魚の記録

    // コンストラクタ（ユーザー名のセット）
    public User(string name)
    {
        this.name = name;
    }

    // データベースへの保存関数
    public void Save()
    {
        if (NetworkManager != null)
        {
            // Userの中身をJSON化したものをFlaskのAPIへPOSTで送信する（ユーザー名、得点、釣り上げた魚の記録）
            string json = JsonUtility.ToJson(this);
            NetworkManager.PostUserData(json);
        }
    }
}