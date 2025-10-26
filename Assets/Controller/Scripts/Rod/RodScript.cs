using System;
using System.Collections.Generic;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Unity.VisualScripting;

#pragma warning disable CS0436 // 型がインポートされた型と競合しています
[RequireComponent(typeof(AudioSource))]
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
    private AudioSource audioSource;
    private AudioClip hauledUpSound;
    private AudioClip biteWateringSound;
    private AudioClip biteThrowSound;
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
    private int stageStyle = 0;

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
        audioSource = GetComponent<AudioSource>();
        hauledUpSound = Resources.Load<AudioClip>("Sounds/hauled_up");
        biteWateringSound = Resources.Load<AudioClip>("Sounds/bite_watering");
        biteThrowSound = Resources.Load<AudioClip>("Sounds/bite_throw");
        audioSource.playOnAwake = false;
        audioSource.loop = false;
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

    public void SetStageStyle(int stageStyle)
    {
        this.stageStyle = stageStyle;
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
                    audioSource.Stop();
                    audioSource.clip = biteThrowSound;
                    audioSource.loop = false;
                    audioSource.Play();
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
        audioSource.Stop();
        audioSource.clip = biteWateringSound;
        audioSource.Play();
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

    bool isBiteWatered = false;
    void FixedUpdate()
    {
        try
        {
            if (isBattle)
            {
                if (thing.IsDestroyed())
                {
                    isBattle = false;
                    biteScript.OutBattle();
                    maxMagnitude = -1;
                    isThrowing = false;
                    return;
                }
                // プレイヤーの操作によって魚の体力を削る処理
                thingStrength -= Mathf.Abs(rotations[id]) * power * Time.fixedDeltaTime;

                // 魚の力によって釣り竿の耐久力を削る処理
                rodStrength -= thing.GetPower * Time.fixedDeltaTime;

                // 釣りバトル勝利時
                if (thingStrength <= 0)
                {
                    Reset(); // バトル状態のリセット

                    thing.Lose(); // 魚側のLose関数を呼び出す
                    strengthPublisher.PublishStrength(id, 0); // ROSへの通知
                    uiScript.ShowReward(thing); // 画面上のUIに釣果を表示させる
                    audioSource.Stop();
                    audioSource.clip = hauledUpSound;
                    audioSource.Play();

                    if (user != null)
                    {
                        // 釣り上げに成功したユーザーのデータコンテナを更新する
                        user.point += thing.GetPoint; // 魚のポイントを加算
                        user.fishedThingNames.Add(thing.GetObjectName); // 魚の名前をリストに追加して記録
                    }
                }

                // 釣りバトル敗北時
                else if (rodStrength <= 0)
                {
                    Reset(); // バトル状態のリセット

                    thing.SetStrength((int)thingStrength); // 魚の残り体力は減った状態で保存して継続する
                    thing.Win(); // 魚側のWin関数を呼び出す
                    strengthPublisher.PublishStrength(id, 0); // ROSへの通知
                    uiScript.ShowSimpleMessage("逃げられた...", 5); // // 画面上のUIにメッセージを表示
                }
            }
        }
        catch (KeyNotFoundException) { }

        if (bite.transform.position.y <= 1f && !isBiteWatered)
        {
            isBiteWatered = true;
            audioSource.Stop();
            audioSource.clip = biteWateringSound;
            audioSource.Play();
        } else if (bite.transform.position.y > 4f)
        {
            isBiteWatered = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetOrientation();
        CheckFeed();

        if (stageStyle == 0)
        {
            if (transform.position.x <= 0) UI.transform.eulerAngles = new Vector3(90, 90, 0);
            else UI.transform.eulerAngles = new Vector3(90, -90, 0);
        }
        else
        {
            if (transform.position.z <= 0) UI.transform.eulerAngles = new Vector3(90, 0, 0);
            else UI.transform.eulerAngles = new Vector3(90, 180, 0);
        }

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