using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    [SerializeField]
    GameObject CT;
    [SerializeField]
    GameObject ID;

    public void SetVisibleCTText(bool visible)
    {
        CT.SetActive(visible);
    }

    public void SetID(string id)
    {
        ID.transform.GetChild(0).GetComponent<TMP_Text>().text = "id: " + id;
    }

    public void SetVisibleIDText(bool visible)
    {
        ID.SetActive(visible);
    }
}
