using UnityEngine;

public class InWaterManager : MonoBehaviour
{

    // 着水判定

    private ThingsToFish fish;

    void Start()
    {
        fish = GetComponent<ThingsToFish>();
    }

    void Update()
    {
        // スペースキーを押したら着水
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fish.SetIsInWater();
            Debug.Log("着水しました。");
        }
    }

}
