using Newtonsoft.Json;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Std;

#pragma warning disable CS0436 // 型がインポートされた型と競合しています
namespace RosSharp.RosBridgeClient.MessageTypes.Sensor
{
    public class Imu : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "sensor_msgs/Imu"; // ROS1の場合はこちら

        [JsonProperty("header")]
        public Header header { get; set; }

        [JsonProperty("orientation")]
        public Quaternion orientation { get; set; }

        [JsonProperty("orientation_covariance")]
        public double[] orientation_covariance { get; set; }

        [JsonProperty("angular_velocity")]
        public Vector3 angular_velocity { get; set; }

        [JsonProperty("angular_velocity_covariance")]
        public double[] angular_velocity_covariance { get; set; }

        [JsonProperty("linear_acceleration")]
        public Vector3 linear_acceleration { get; set; }

        [JsonProperty("linear_acceleration_covariance")]
        public double[] linear_acceleration_covariance { get; set; }

        public Imu()
        {
            header = new Header();
            orientation = new Quaternion();
            orientation_covariance = new double[9];
            angular_velocity = new Vector3();
            angular_velocity_covariance = new double[9];
            linear_acceleration = new Vector3();
            linear_acceleration_covariance = new double[9];
        }

        public Imu(Header header,
            Quaternion orientation,
            double[] orientation_covariance,
            Vector3 angular_velocity,
            double[] angular_velocity_covariance,
            Vector3 linear_acceleration,
            double[] linear_acceleration_covariance)
        {
            this.header = header;
            this.orientation = orientation;
            this.orientation_covariance = orientation_covariance;
            this.angular_velocity = angular_velocity;
            this.angular_velocity_covariance = angular_velocity_covariance;
            this.linear_acceleration = linear_acceleration;
            this.linear_acceleration_covariance = linear_acceleration_covariance;
        }
    }
}
#pragma warning restore CS0436 // 型がインポートされた型と競合しています