using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using RosSharp;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

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
    [SerializeField]
    private float throwPower = 0.3f;
    [SerializeField]
    private float power = 1f;
    [SerializeField]
    private float maxRodStrength = 100f;
    [SerializeField]
    private GameObject UI;
    [SerializeField]
    private LineRenderer line;
    [SerializeField]
    private GameObject tip;
    private UIScript uiScript;
    private float baseRotationY = 0;
    private string id = "";
    private float maxMagnitude = -1;
    private DateTime rodTime = DateTime.Now;
    private float coolTime = 6;
    private bool isThrowing = false;
    private bool isThrown = false;
    private bool coolDown = false;
    private float thresholdMagnitude = 5;
    private float throwingTime = 1;
    private bool isBattle = false;
    private BiteScript biteScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imus = imuSubscriber.GetImus();
        rotations = rotationSubscriber.GetRotations();
        biteScript = bite.GetComponent<BiteScript>();
        uiScript = UI.GetComponent<UIScript>();
        uiScript.SetID(id);
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

    void ClearCoolDown()
    {
        coolDown = false;
        uiScript.SetVisibleCTText(false);
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
        if (isBattle) return;
        try
        {
            Vector3 accel = new Vector3((float)imus[id].linear_acceleration.x, (float)imus[id].linear_acceleration.y, (float)imus[id].linear_acceleration.z);
            if (accel.magnitude > thresholdMagnitude && !coolDown)
            {
                if (!isThrowing)
                {
                    rodTime = DateTime.Now;
                    isThrowing = true;
                }
                maxMagnitude = Math.Max(maxMagnitude, accel.magnitude);
            }
            if (((DateTime.Now - rodTime).TotalSeconds > throwingTime) && isThrowing && !coolDown)
            {
                if (!isThrown)
                {
                    isThrowing = false;
                    Vector3 biteVelocity = rod.transform.up;
                    biteVelocity.y = 0;
                    biteVelocity = biteVelocity.normalized * -(maxMagnitude - thresholdMagnitude) * throwPower;
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
                    maxMagnitude = -1;
                    isThrown = false;
                    isThrowing = false;
                }
                coolDown = true;
                uiScript.SetVisibleCTText(true);
                Invoke("ClearCoolDown", coolTime);
            }
        }
        catch (KeyNotFoundException) { }
    }


    private float thingStrength = 0;
    private float rodStrength = 0;
    private ThingsToFish thing;
    public void InBattle(ThingsToFish thing)
    {
        if (isBattle) return;
        thing.InBattle();
        isBattle = true;

        thingStrength = thing.GetStrength;
        rodStrength = maxRodStrength;
        this.thing = thing;

        strengthPublisher.PublishStrength(id, thing.GetPower);
    }

    void FixedUpdate()
    {
        try
        {
            if (isBattle)
            {
                thingStrength -= rotations[id] * power * Time.fixedDeltaTime;
                rodStrength -= thing.GetPower * Time.fixedDeltaTime;

                if (thingStrength <= 0)
                {
                    isBattle = false;
                    biteScript.OutBattle();
                    bite.SetActive(false);
                    bite.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    bite.transform.position = transform.position + new Vector3(0, 5, 0);
                    maxMagnitude = -1;
                    isThrown = false;
                    isThrowing = false;

                    thing.Lose();
                    strengthPublisher.PublishStrength(id, 0);
                }
                else if (rodStrength <= 0)
                {
                    isBattle = false;
                    biteScript.OutBattle();
                    bite.SetActive(false);
                    bite.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    bite.transform.position = transform.position + new Vector3(0, 5, 0);
                    maxMagnitude = -1;
                    isThrown = false;
                    isThrowing = false;

                    thing.Win();
                    strengthPublisher.PublishStrength(id, 0);
                }
            }
        } catch (KeyNotFoundException) {}
    }

    // Update is called once per frame
    void Update()
    {
        SetOrientation();
        CheckFeed();

        if (transform.position.x <= 0) UI.transform.eulerAngles = new Vector3(90, 90, 0);
        else UI.transform.eulerAngles = new Vector3(90, -90, 0);

        line.positionCount = 1;
        if (bite.activeSelf)
        {
            line.positionCount = 2;
            line.SetPosition(1, bite.transform.position);
        }
        line.SetPosition(0, tip.transform.position);
    }
}
