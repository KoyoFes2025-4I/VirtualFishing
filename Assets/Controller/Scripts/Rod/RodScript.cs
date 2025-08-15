using System;
using System.Collections.Generic;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

#pragma warning disable CS0436 // 型がインポートされた型と競合しています
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
    [SerializeField]
    private BiteScript biteScript;
    private UIScript uiScript;
    private float uiScale = 1f;
    private float baseRotationY = 0;
    private string id = "";
    private float maxMagnitude = -1;
    private DateTime rodTime = DateTime.Now;
    private float maxCoolTime = 6;
    private bool isThrowing = false;
    private bool isThrown = false;
    private float coolTime = 0f;
    private float thresholdMagnitude = 5;
    private float throwingTime = 1;
    private bool isBattle = false;

    public BiteScript GetBiteScript => biteScript;
    private User user;

    void Start()
    {
        imus = imuSubscriber.GetImus();
        rotations = rotationSubscriber.GetRotations();
        uiScript = UI.transform.GetChild(0).GetComponent<UIScript>();
        if (user != null) uiScript.SetID(id, user.name);
        else uiScript.SetID(id);
        uiScript.SetScale(uiScale);
    }

    public void SetId(string id)
    {
        this.id = id;
        try { uiScript.SetID(id); }
        catch (NullReferenceException) { }
    }
    public void SetUIScale(float scale)
    {
        uiScale = scale;
        try { uiScript.SetScale(scale); }
        catch (NullReferenceException) { }
    }
    public void SetThrowPower(float throwPower)
    {
        this.throwPower = throwPower;
    }

    public void SetPower(float power)
    {
        this.power = power;
    }

    public void SetMaxRodStrength(float maxRodStrength)
    {
        this.maxRodStrength = maxRodStrength;
    }
    public void SetUser(User user)
    {
        this.user = user;
        try { uiScript.SetID(id, user.name); }
        catch (NullReferenceException) { }
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
        }
        catch (KeyNotFoundException)
        {
            uiScript.SetIDItalic();
        }
    }

    void CheckFeed()
    {
        if (isBattle) return;
        try
        {
            Vector3 accel = new Vector3((float)imus[id].linear_acceleration.x, (float)imus[id].linear_acceleration.y, (float)imus[id].linear_acceleration.z);
            if (accel.magnitude > thresholdMagnitude && coolTime <= 0f)
            {
                if (!isThrowing)
                {
                    rodTime = DateTime.Now;
                    isThrowing = true;
                }
                maxMagnitude = Math.Max(maxMagnitude, accel.magnitude);
            }
            if (((DateTime.Now - rodTime).TotalSeconds > throwingTime) && isThrowing && coolTime <= 0f)
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
                coolTime = maxCoolTime;
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

    public void Reset()
    {
        isBattle = false;
        biteScript.OutBattle();
        bite.SetActive(false);
        bite.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        bite.transform.position = transform.position + new Vector3(0, 5, 0);
        maxMagnitude = -1;
        isThrown = false;
        isThrowing = false;
        coolTime = maxCoolTime;
    }

    public void ShowMessage(string message, float duration)
    {
        uiScript.ShowSimpleMessage(message, duration);
    }

    public void ShowResult()
    {
        if (user == null) return;
        uiScript.ShowResult(user);
    }

    void FixedUpdate()
    {
        try
        {
            if (isBattle)
            {
                thingStrength -= Mathf.Abs(rotations[id]) * power * Time.fixedDeltaTime;
                rodStrength -= thing.GetPower * Time.fixedDeltaTime;

                if (thingStrength <= 0)
                {
                    Reset();

                    thing.Lose();
                    strengthPublisher.PublishStrength(id, 0);
                    uiScript.ShowReward(thing);

                    if (user != null)
                    {
                        user.point += thing.GetPoint;
                        user.fishedThingNames.Add(thing.GetObjectName);
                    }
                }
                else if (rodStrength <= 0)
                {
                    Reset();

                    thing.Win();
                    strengthPublisher.PublishStrength(id, 0);
                    uiScript.ShowSimpleMessage("逃げられた...", 5);
                }
            }
        }
        catch (KeyNotFoundException) { }
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

        uiScript.SetVisibleCTText(coolTime > 0f);
        if (coolTime > 0f) coolTime -= Time.deltaTime;
    }
}

#pragma warning restore CS0436 // 型がインポートされた型と競合しています