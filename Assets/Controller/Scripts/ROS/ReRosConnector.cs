using System.Threading;
using RosSharp.RosBridgeClient;
using WebSocketSharp;

public class ReRosConnector : RosConnector
{
    public int version = 0;
    public void SetRosBridgeServerUrl(string url)
    {
        if (RosBridgeServerUrl == url || url.IsNullOrEmpty()) return;
        RosBridgeServerUrl = url;
        RosSocket.Close();
        new Thread(ConnectAndWait).Start();
        version++;
    }
}