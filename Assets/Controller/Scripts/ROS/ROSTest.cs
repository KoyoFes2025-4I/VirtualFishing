using System.Collections.Generic;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;

public class ROSTest : MonoBehaviour
{
    private Dictionary<string, Imu> imus;
    private Dictionary<string, float> rotations;
    [SerializeField]
    private IMUSubscriber imuSubscriber;
    [SerializeField]
    private RotationSubscriber rotationSubscriber;
    [SerializeField]
    private StrengthPublisher strengthPublisher;
    [SerializeField]
    GameObject rod;
    [SerializeField]
    float baseRotationY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imuSubscriber = GetComponent<IMUSubscriber>();
        rotationSubscriber = GetComponent<RotationSubscriber>();
        strengthPublisher = GetComponent<StrengthPublisher>();
        imus = imuSubscriber.GetImus();
        rotations = rotationSubscriber.GetRotations();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            Quaternion zRotation = Quaternion.AngleAxis(90f, Vector3.forward);
            Quaternion yRotation = Quaternion.AngleAxis(baseRotationY, Vector3.up);
            rod.transform.rotation = yRotation * zRotation * new Quaternion((float)imus["katsu"].orientation.x, (float)-imus["katsu"].orientation.y, (float)imus["katsu"].orientation.z, (float)imus["katsu"].orientation.w);
        } 
        catch
        {
            Debug.Log("none");
        }
    }
}
