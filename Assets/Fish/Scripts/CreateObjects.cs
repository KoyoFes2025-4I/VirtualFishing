using UnityEngine;

public class CreateObjects : MonoBehaviour
{

    // プレハブ化した各々のFishオブジェクトを条件に従って生成するようにする

    [SerializeField] private Vector3 prefab_position1; // 配置する初期座標
    [SerializeField] private Vector3 prefab_position2;
    [SerializeField] private Vector3 prefab_position3;
    [SerializeField] GameObject prefab1;
    [SerializeField] GameObject prefab2;
    [SerializeField] GameObject prefab3;

    // オブジェクトの種類の分だけプレハブを定義する

    void Start()
    {

        // ポップのアルゴリズムを考えて各条件を設定する

        for (int i = 0; i < 2; i++)
        {
            Instantiate(prefab1, prefab_position1, Quaternion.identity);
            Instantiate(prefab2, prefab_position2, Quaternion.identity);
            Instantiate(prefab3, prefab_position3, Quaternion.identity);
        }

    }

}
