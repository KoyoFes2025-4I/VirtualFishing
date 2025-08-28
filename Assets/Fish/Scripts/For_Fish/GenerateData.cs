using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GenerateData
{
    public List<ThingData> thingDatas;
    public int keepThingCount = 5;
    public float spawnMargin = 3f;
}

[Serializable]
public class ThingData
{
    public GameObject prefab;
    public float weight = 1f;
}