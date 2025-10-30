using System.Threading;
using RosSharp.RosBridgeClient;
using WebSocketSharp;

public class ReRosConnector : RosConnector
{
    public int version = 0;
    public void SetRosBridgeServerUrl(string url)
    {
        // ROS–¢Ú‘±‚ÌNullReferenceException‰ñ”ğ
        if (RosBridgeServerUrl == url || url.IsNullOrEmpty() || IsConnected == null || RosSocket == null) return;

        RosBridgeServerUrl = url;
        RosSocket.Close();
        new Thread(ConnectAndWait).Start();
        version++;
    }
}