using UnityEngine;

public class CreateObjects : MonoBehaviour
{

    // プレハブ化した各々のFishオブジェクトを条件に従って生成するようにする

    [SerializeField] private Vector3 prefab_position;// 配置する座標
    [SerializeField] GameObject prefab1;
    [SerializeField] GameObject prefab2;
    // オブジェクトの種類の分だけプレハブを定義する

    void Start()
    {

        // ポップのアルゴリズムを考えて各条件を設定する

        for (int i = 0; i < 2; i++)
        {
            Instantiate(prefab1, prefab_position, Quaternion.identity);
        }

        for (int j = 0; j < 2; j++)
        {
            Instantiate(prefab2, prefab_position, Quaternion.identity);
        }

    }

}
