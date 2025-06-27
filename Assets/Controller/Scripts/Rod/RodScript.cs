using System;
using System.Collections.Generic;
using RosSharp;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;

public class RodScript : MonoBehaviour
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
    float baseRotationY;
    [SerializeField]
    private GameObject ball;
    private string id = "";
    private float maxMagunitude;
    private DateTime rodTime;
    private bool isThrowing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imus = imuSubscriber.GetImus();
        rotations = rotationSubscriber.GetRotations();
    }

    void SetId(string id)
    {
        this.id = id;
    }

    void Checkfeed()
    {
        Vector3 accel = new Vector3((float)imus[id].linear_acceleration.x, (float)imus[id].linear_acceleration.y, (float)imus[id].linear_acceleration.z);
        Debug.Log(accel.magnitude);
        if(accel.magnitude > 5)
        {
            if(!isThrowing){
                rodTime = DateTime.Now;
                isThrowing = true;
            }
            if (maxMagunitude < accel.magnitude)
            {
                maxMagunitude = accel.magnitude;
            }
        }
        if ((((DateTime.Now - rodTime).TotalSeconds > 1) && (isThrowing)))
        {
            isThrowing = false;
            Vector3 bite = transform.up;
            bite.y = 0;
            bite = bite.normalized * -maxMagunitude;
            ball.SetActive(true);
            ball.GetComponent<Rigidbody>().linearVelocity = bite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            Quaternion zRotation = Quaternion.AngleAxis(90f, Vector3.forward);
            Quaternion yRotation = Quaternion.AngleAxis(baseRotationY, Vector3.up);
            transform.rotation = yRotation * zRotation * new Quaternion((float)imus[id].orientation.x, (float)-imus[id].orientation.y, (float)imus[id].orientation.z, (float)imus[id].orientation.w);
            Checkfeed();
        }
        catch
        {
            Debug.Log("none");
        }
    }
}
