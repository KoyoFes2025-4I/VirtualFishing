using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThingGenerator : MonoBehaviour
{
    [SerializeField]
    private List<GenerateData> generateDatas;
    [SerializeField]
    private int generateDataIndex;
    [SerializeField]
    StageManager stageManager;

    private List<GameObject> things = new List<GameObject>();
    public bool isGenerate = false;

    void Start()
    {

    }

    void Update()
    {
        List<GameObject> tmp = new List<GameObject>(things);
        for (int i = 0; i < tmp.Count; i++)
        {
            if (tmp[i].IsDestroyed()) things.RemoveAt(i);
        }

        if (generateDatas[generateDataIndex].keepThingCount > things.Count && isGenerate) Generate();
    }

    public void Generate()
    {
        GameObject prefab = null;
        Vector3 spawnPos = new Vector3(0, -1, 0);

        float totalWeight = 0f;
        foreach (ThingData data in generateDatas[generateDataIndex].thingDatas) totalWeight += data.weight;
        float weight = Random.Range(0f, totalWeight);
        foreach (ThingData data in generateDatas[generateDataIndex].thingDatas)
        {
            if (weight <= data.weight)
            {
                prefab = data.prefab;
                break;
            }
            weight -= data.weight;
        }

        spawnPos.x = Random.Range(-stageManager.width / 2f + generateDatas[generateDataIndex].spawnMargin, stageManager.width / 2f - generateDatas[generateDataIndex].spawnMargin);
        spawnPos.z = Random.Range(-stageManager.height / 2f + generateDatas[generateDataIndex].spawnMargin, stageManager.height / 2f - generateDatas[generateDataIndex].spawnMargin);

        ThingsToFish thing = Instantiate(prefab, spawnPos, Quaternion.identity, transform).GetComponent<ThingsToFish>();
        thing.name = thing.GetObjectName;
        thing.gameObject.SetActive(true);
        things.Add(thing.gameObject);
    }

    public void Regenerate()
    {
        foreach (GameObject thing in things) Destroy(thing);
        things.Clear();
    }
}
