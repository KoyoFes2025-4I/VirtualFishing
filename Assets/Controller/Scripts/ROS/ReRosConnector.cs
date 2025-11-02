using System.Threading;
using RosSharp.RosBridgeClient;
using WebSocketSharp;

public class ReRosConnector : RosConnector
{
    public int version = 0;

    public override void Awake()
    {
    }
    public void SetRosBridgeServerUrl(string url)
    {
        // ROS���ڑ�����NullReferenceException���
        if (url.IsNullOrEmpty()) return;
        RosBridgeServerUrl = url;

        if (IsConnected == null)
        {
            base.Awake();
        } else
        {
            if (RosSocket != null) RosSocket.Close();
            new Thread(ConnectAndWait).Start();
        }
        
        version++;
    }
}