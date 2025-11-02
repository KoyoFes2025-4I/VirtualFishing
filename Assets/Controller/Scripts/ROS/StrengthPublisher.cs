using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;


/// <summary>
/// 釣り竿の持ち手の回転に対する抵抗を出力するためのクラス
/// </summary>
public class StrengthPublisher : UnityPublisher<String>
{
    private int version = 0;
    protected override void Start()
    {
    }

    /// <summary>
    /// 釣り竿の持ち手の回転に対する抵抗を設定する
    /// </summary>
    /// <param name="id">
    /// 釣り竿のID
    /// </param>
    /// <param name="strength">
    /// 抵抗の強さ
    /// </param>
    public void PublishStrength(string id, float strength)
    {
        String msg = new String();
        msg.data = $"{id},{strength}";
        Publish(msg);
    }

    void Update()
    {
        if (GetComponent<ReRosConnector>().version != version)
        {
            version = GetComponent<ReRosConnector>().version;
            if (GetComponent<RosConnector>().IsConnected.WaitOne(1000)) base.Start();
        }
    }
}