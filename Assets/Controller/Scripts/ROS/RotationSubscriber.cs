using System;
using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using UnityEngine;
using String = RosSharp.RosBridgeClient.MessageTypes.Std.String;

/// <summary>
/// 釣り竿の持ち手の部分の回転速度を取得するためのクラス
/// </summary>
public class RotationSubscriber : UnitySubscriber<String>
{
    private Dictionary<string, float> rotations = new Dictionary<string, float>();
    private Dictionary<string, DateTime> timestamp = new Dictionary<string, DateTime>();
    protected override void Start()
    {
        base.Start();
    }
    protected override void ReceiveMessage(String message)
    {
        string[] strings = message.data.Split(",");
        try
        {
            rotations[strings[0]] = float.Parse(strings[1]);
            timestamp[strings[0]] = DateTime.Now;
        }
        catch
        {
            Debug.LogWarning("回転速度取得失敗");
        }
    }

    void Update()
    {
        CheckTimeout();
    }

    /// <summary>
    /// 一定時間入力がなかったキーを削除するための関数
    /// </summary>
    private void CheckTimeout()
    {
        Dictionary<string, float> tmp = new Dictionary<string, float>(rotations);
        foreach (string key in tmp.Keys)
        {
            if (DateTime.Now >= timestamp[key].AddSeconds(5))
            {
                rotations.Remove(key);
                timestamp.Remove(key);
            }
        }
    }

    /// <summary>
    /// 釣り竿の持ち手の回転速度を格納したDictionaryを返す関数
    /// 一定時間入力がなかったものは自動で削除される
    /// </summary>
    /// <returns>
    /// キーがIDの回転速度のDictionary
    /// </returns>
    public Dictionary<string, float> GetRotations()
    {
        return rotations;
    }
}
