using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using System.Collections.Generic;
using System;

/// <summary>
/// IMUの入力を取得するためのクラス
/// </summary>
public class IMUSubscriber : UnitySubscriber<Imu>
{
    private Dictionary<string, Imu> imus = new Dictionary<string, Imu>();
    private Dictionary<string, DateTime> timestamp = new Dictionary<string, DateTime>();
    protected override void ReceiveMessage(Imu imu)
    {
        imus[imu.header.frame_id] = imu;
        timestamp[imu.header.frame_id] = DateTime.Now;
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        CheckTimeout();
    }

    /// <summary>
    /// 一定時間IMUの入力がなかった場合にキーを削除するための関数
    /// </summary>
    private void CheckTimeout()
    {
        Dictionary<string, Imu> tmp = new Dictionary<string, Imu>(imus);
        foreach (string key in tmp.Keys)
        {
            if (DateTime.Now >= timestamp[key].AddSeconds(5))
            {
                imus.Remove(key);
                timestamp.Remove(key);
            }
        }
    }
    /// <summary>
    /// 取得したIMUの入力を返す関数
    /// 一定時間入力がなかったIMUは自動で削除される
    /// </summary>
    /// <returns>
    /// キーがIDのIMUのDictionary
    /// </returns>
    public Dictionary<string, Imu> GetImus()
    {
        return imus;
    }
}
