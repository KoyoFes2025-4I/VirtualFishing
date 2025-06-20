using UnityEngine;

public class InWaterManager : MonoBehaviour
{

    // 着水判定

    private ThingsToFish fish;

    void Update()
    {
        // スペースキーを押したら着水
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fish.SetIsInWater();
        }
    }

}
