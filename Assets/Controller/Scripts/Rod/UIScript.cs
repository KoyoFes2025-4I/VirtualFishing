using System.Threading.Tasks;
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

    private TMP_Text idText;

    public void SetVisibleCTText(bool visible)
    {
        ct.SetActive(visible);
    }

    public void SetID(string id)
    {
        idText.text = "id: " + id;
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

    private ThingsToFish rewardThing;
    public void ShowReward(ThingsToFish thing)
    {
        creator.text = "作: " + thing.GetCreator;
        fishName.text = thing.GetObjectName;
        point.text = "ポイント: " + thing.GetPoint;
        reward.SetActive(true);
        rewardThing = thing;
        thing.transform.position = this.thing.transform.position;
        thing.transform.eulerAngles = new Vector3(90, -90, 0);
        thing.transform.localScale = Vector3.one;
        thing.gameObject.SetActive(true);
        Invoke(nameof(HideReward), 5);
    }

    public void HideReward()
    {
        reward.SetActive(false);
        Destroy(rewardThing.gameObject);
    }

    private float simpleMessageDuration = 0f;
    public void ShowSimpleMessage(string msg, float duration)
    {
        simpleMessageDuration = duration;
        simpleMessage.text = msg;
        simpleMessage.gameObject.SetActive(true);
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
