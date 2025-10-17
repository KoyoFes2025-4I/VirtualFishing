using System;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    [SerializeField]
    GameObject ct;
    [SerializeField]
    GameObject id;
    [SerializeField]
    GameObject reward;
    [SerializeField]
    TMP_Text creator;
    [SerializeField]
    TMP_Text fishName;
    [SerializeField]
    TMP_Text point;
    [SerializeField]
    GameObject thing;
    [SerializeField]
    TMP_Text simpleMessage;
    [SerializeField]
    TMP_Text resultMessage;

    private TMP_Text idText;

    public void SetVisibleCTText(bool visible)
    {
        ct.SetActive(visible);
    }

    public void SetID(string id, string name = null)
    {
        idText.text = "id: " + id;
        if (name != null) idText.text += ", name: " + name;
    }

    private float idItalicDuration = 0f;
    public void SetIDItalic(float duration = 0.3f)
    {
        idItalicDuration = duration;
        idText.fontStyle = FontStyles.Italic;
    }

    public void SetVisibleIDText(bool visible)
    {
        id.SetActive(visible);
    }

    public void ShowResult(User user)
    {
        HideReward();
        simpleMessageDuration = 0f;
        resultMessage.text = $"なまえ：{user.name}\nポイント：{user.point}\n釣った魚の種類：{user.fishedThingNames.Count}";
        resultMessage.gameObject.SetActive(true);
    }

    private ThingsToFish rewardThing;
    public void ShowReward(ThingsToFish thing)
    {
        creator.text = "作: " + thing.GetCreator;
        fishName.text = thing.GetObjectName;
        point.text = "ポイント: " + thing.GetPoint;
        reward.SetActive(true);
        rewardThing = thing;
        thing.transform.parent = this.thing.transform;
        thing.transform.position = this.thing.transform.position;
        thing.transform.localEulerAngles = new Vector3(0, 0, 0);
        thing.transform.localScale = transform.localScale;
        thing.gameObject.SetActive(true);
        Invoke(nameof(HideReward), 5);
    }

    public void HideReward()
    {
        reward.SetActive(false);
        try { Destroy(rewardThing.gameObject); }
        catch (MissingReferenceException) {} catch (NullReferenceException) {}
    }

    private float simpleMessageDuration = 0f;
    public void ShowSimpleMessage(string msg, float duration)
    {
        simpleMessageDuration = duration;
        simpleMessage.text = msg;
        simpleMessage.gameObject.SetActive(true);
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    void Awake()
    {
        idText = id.transform.GetChild(0).GetComponent<TMP_Text>();
    }
    void Update()
    {
        if (simpleMessageDuration > 0f) simpleMessageDuration -= Time.deltaTime;
        else simpleMessage.gameObject.SetActive(false);

        if (idItalicDuration > 0f) idItalicDuration -= Time.deltaTime;
        else idText.fontStyle = FontStyles.Normal;
    }
}
