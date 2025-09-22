using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GenerateData
{
    public List<ThingData> thingDatas; // 生成対象となるオブジェクトのPrefabのリスト
    public int keepThingCount = 5; // オブジェクトの同時ポップ数（常に維持する）
    public float spawnMargin = 3f; // 水域の端から生成される位置までの余白
}

[Serializable]
public class ThingData
{
    public GameObject prefab; // 実際に生成するPrefab
    public float weight = 1f; // 生成される確率の重みづけ（大きいほど生成されやすい）
}