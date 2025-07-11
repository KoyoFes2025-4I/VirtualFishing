using System;
using System.Collections.Generic;
using System.Data;
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
    private GameObject rod;
    [SerializeField]
    private GameObject bite;
    private float baseRotationY = 0;
    private string id = "";
    private float maxMagnitude = -1;
    private DateTime rodTime = DateTime.Now;
    private bool isThrowing = false;
    private bool isThrown = false;
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

    public void SetThresholdMagnitude(float magnitude)
    {
        thresholdMagnitude = magnitude;
    }

    public void SetThrowingTime(float time)
    {
        throwingTime = time;
    }

    void SetOrientation()
    {
        try
        {
            Quaternion zRotation = Quaternion.AngleAxis(90f, Vector3.forward);
            Quaternion yRotation = Quaternion.AngleAxis(baseRotationY, Vector3.up);
            rod.transform.rotation = yRotation * zRotation * new Quaternion((float)imus[id].orientation.x, (float)-imus[id].orientation.y, (float)imus[id].orientation.z, (float)imus[id].orientation.w);
        } catch (Exception) {}
    }

    void CheckFeed()
    {
        try
        {
            Vector3 accel = new Vector3((float)imus[id].linear_acceleration.x, (float)imus[id].linear_acceleration.y, (float)imus[id].linear_acceleration.z);
            if (accel.magnitude > thresholdMagnitude)
            {
                if (!isThrowing)
                {
                    rodTime = DateTime.Now;
                    isThrowing = true;
                }
                maxMagnitude = Math.Max(maxMagnitude, accel.magnitude);
            }
            if (((DateTime.Now - rodTime).TotalSeconds > throwingTime) && isThrowing)
            {
                if (!isThrown)
                {
                    isThrowing = false;
                    Vector3 biteVelocity = transform.up;
                    biteVelocity.y = 0;
                    biteVelocity = biteVelocity.normalized * -maxMagnitude;
                    bite.SetActive(true);
                    bite.GetComponent<Rigidbody>().linearVelocity = biteVelocity;
                    maxMagnitude = -1;
                    isThrown = true;
                }
                else
                {
                    bite.SetActive(false);
                    bite.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    bite.transform.position = transform.position + new Vector3(0, 5, 0);
                    isThrown = false;
                }
            }
        }
        catch (Exception) { }
    }

    // Update is called once per frame
    void Update()
    {
        SetOrientation();
        CheckFeed();
    }
}
