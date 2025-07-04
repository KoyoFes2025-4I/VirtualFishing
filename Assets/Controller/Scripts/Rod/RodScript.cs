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
    private GameObject ball;
    private float baseRotationY;
    private string id = "";
    private float maxMagnitude = -1;
    private DateTime rodTime;
    private bool isThrowing;
    private float thresholdMagnitude = 5;
    private float throwingTime = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imus = imuSubscriber.GetImus();
        rotations = rotationSubscriber.GetRotations();
    }

    public void SetId(string id)
    {
        this.id = id;
    }

    public void SetBaseRotationY(float y)
    {
        baseRotationY = y;
    }

    void CheckFeed()
    {
        Vector3 accel = new Vector3((float)imus[id].linear_acceleration.x, (float)imus[id].linear_acceleration.y, (float)imus[id].linear_acceleration.z);
        if(accel.magnitude > thresholdMagnitude)
        {
            if(!isThrowing){
                rodTime = DateTime.Now;
                isThrowing = true;
            }
            if (maxMagnitude < accel.magnitude)
            {
                maxMagnitude = accel.magnitude;
            }
        }
        if (((DateTime.Now - rodTime).TotalSeconds > throwingTime) && isThrowing)
        {
            isThrowing = false;
            Vector3 bite = transform.up;
            bite.y = 0;
            bite = bite.normalized * -maxMagnitude;
            ball.SetActive(true);
            ball.GetComponent<Rigidbody>().linearVelocity = bite;
            maxMagnitude = -1;
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
            CheckFeed();
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
}
