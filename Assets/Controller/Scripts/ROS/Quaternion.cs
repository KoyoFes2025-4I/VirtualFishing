using Newtonsoft.Json;

namespace RosSharp.RosBridgeClient.MessageTypes.Geometry
{
    public class Quaternion : Message
    {
        [JsonProperty("x")]
        public double x { get; set; }

        [JsonProperty("y")]
        public double y { get; set; }

        [JsonProperty("z")]
        public double z { get; set; }

        [JsonProperty("w")]
        public double w { get; set; }

        public Quaternion()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 1; // デフォルトは単位クォータニオン
        }

        public Quaternion(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}
