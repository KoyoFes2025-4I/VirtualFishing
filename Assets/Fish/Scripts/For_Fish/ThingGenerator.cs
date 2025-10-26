using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThingGenerator : MonoBehaviour
{
    [SerializeField] private List<GenerateData> generateDatas; // 生成設定データのリスト
    [SerializeField] StageManager waterScale; // 水域のサイズ情報

    private int generateDataIndex = 0; // 現在使用する生成データのインデックス
    private List<GameObject> things = new List<GameObject>(); // 現在生成されているオブジェクト
    public bool isGenerate = false; // 生成を許可するフラグ

    void Start()
    {

    }

    void Update()
    {
        // 現在の生成オブジェクトをコピーしてチェック
        List<GameObject> tmp = new List<GameObject>(things);
        for (int i = 0; i < tmp.Count; i++)
        {
            if (tmp[i].IsDestroyed()) things.RemoveAt(i); // 破壊済みオブジェクトはリストから削除
        }

        // まだ合計の生成数が足りない場合かつ生成フラグがONなら新規生成する
        // 生成するオブジェクトは畳み付きランダムでGenerate関数内で決定させる
        if (generateDatas[generateDataIndex].keepThingCount > things.Count && isGenerate) Generate();
    }

    // 魚オブジェクト（prefab）のポップ処理（GenerateDataに依存）
    public void Generate()
    {
        GameObject prefab = null;
        Vector3 spawnPos = new Vector3(0, -1, 0); // ポップ初期位置

        // 重み付きランダムで生成するPrefabを複数ある中から決定
        float totalWeight = 0f;
        foreach (ThingData data in generateDatas[generateDataIndex].thingDatas) totalWeight += data.weight;
        float weight = Random.Range(0f, totalWeight);
        foreach (ThingData data in generateDatas[generateDataIndex].thingDatas)
        {
            if (weight <= data.weight)
            {
                prefab = data.prefab; // 生成するPrefabの決定
                break;
            }
            weight -= data.weight;
        }

        // 生成位置を生成範囲内からランダムに決定
        spawnPos.x = Random.Range(-waterScale.width / 2f + generateDatas[generateDataIndex].spawnMargin, waterScale.width / 2f - generateDatas[generateDataIndex].spawnMargin);
        spawnPos.z = Random.Range(-waterScale.height / 2f + generateDatas[generateDataIndex].spawnMargin, waterScale.height / 2f - generateDatas[generateDataIndex].spawnMargin);

        // Prefabを新規生成してThingsToFishコンポーネントを取得させる
        ThingsToFish thing = Instantiate(prefab, spawnPos, Quaternion.identity, transform).GetComponent<ThingsToFish>();
        thing.gameObject.SetActive(true);
        things.Add(thing.gameObject);
    }

    // 現在生成されているオブジェクトを全て破棄するメソッド
    public void Regenerate()
    {
        foreach (GameObject thing in things) Destroy(thing);
        things.Clear();
    }

    public void ChangeGenerateDataIndex(int index)
    {
        if (generateDatas.Count > index) generateDataIndex = index;
    }
}
