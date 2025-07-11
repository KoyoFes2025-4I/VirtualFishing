using UnityEngine;

public class BiteScript : MonoBehaviour
{
    [Header("浮力の設定")]
    public float waterLevel = 0f;       // 水面のY座標
    public float buoyancyForce = 10f;   // 浮力の最大強度（完全に水没時）
    public float fluidDensity = 1000f;  // 流体（水）の密度（例: 水は1000kg/m^3）

    private Rigidbody rb;
    private Collider objCollider;       // オブジェクトのCollider
    private float objectVolume;         // オブジェクトの近似体積
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objCollider = GetComponent<Collider>(); // Colliderを取得

        if (rb == null)
        {
            Debug.LogError("Rigidbodyがアタッチされていません。BuoyancyAdvancedスクリプトはRigidbodyが必要です。");
            enabled = false;
            return;
        }

        if (objCollider == null)
        {
            Debug.LogError("Colliderがアタッチされていません。BuoyancyAdvancedスクリプトはColliderが必要です。");
            enabled = false;
            return;
        }

        Debug.Log("test");

        // オブジェクトの近似体積を計算 (Boundsのサイズから)
        // これは非常に簡略化された体積であり、正確なメッシュ体積ではない点に注意
        objectVolume = objCollider.bounds.size.x * objCollider.bounds.size.y * objCollider.bounds.size.z;
    }

    void FixedUpdate()
    {
        //if (rb == null || objCollider == null) return;

        // オブジェクトの境界ボックスを取得
        Bounds bounds = objCollider.bounds;

        // 水面下にある境界ボックスの最低点
        float submergedBottom = Mathf.Max(waterLevel, bounds.min.y);
        // 水面下にある境界ボックスの最高点
        float submergedTop = Mathf.Min(waterLevel, bounds.max.y);

        // 水面下にある部分の高さ
        float submergedHeight = submergedTop - submergedBottom;

        // オブジェクトが水面下にあるか判定
        if (submergedHeight > 0)
        {
            // 水面下にある垂直方向の割合 (0.0 から 1.0)
            float submergedRatioY = submergedHeight / bounds.size.y;

            // 水面下にある部分の体積の割合を推定
            // これは境界ボックスを基準とした近似であり、正確な部分体積ではない
            float submergedVolumeRatio = submergedRatioY; // Y軸方向の割合を体積割合として扱う

            // アルキメデスの原理に基づく浮力の計算
            // 浮力 = 流体の密度 × 沈んでいる部分の体積 × 重力加速度
            // UnityのAddForceでは質量で割る必要がないため、そのまま力として適用
            float effectiveBuoyancyForce = fluidDensity * (objectVolume * submergedVolumeRatio) * Physics.gravity.magnitude;

            // Debug.Log($"Submerged Volume Ratio: {submergedVolumeRatio:F2}, Effective Buoyancy Force: {effectiveBuoyancyForce:F2}");

            // 上向きの力を加える
            rb.AddForce(Vector3.up * effectiveBuoyancyForce, ForceMode.Force);

            Debug.Log(Vector3.up * effectiveBuoyancyForce);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (rb == null || objCollider == null) return;

        // オブジェクトの境界ボックスを取得
        Bounds bounds = objCollider.bounds;

        // 水面下にある境界ボックスの最低点
        float submergedBottom = Mathf.Max(waterLevel, bounds.min.y);
        // 水面下にある境界ボックスの最高点
        float submergedTop = Mathf.Min(waterLevel, bounds.max.y);

        // 水面下にある部分の高さ
        float submergedHeight = submergedTop - submergedBottom;

        // オブジェクトが水面下にあるか判定
        if (submergedHeight > 0)
        {
            // 水面下にある垂直方向の割合 (0.0 から 1.0)
            float submergedRatioY = submergedHeight / bounds.size.y;

            // 水面下にある部分の体積の割合を推定
            // これは境界ボックスを基準とした近似であり、正確な部分体積ではない
            float submergedVolumeRatio = submergedRatioY; // Y軸方向の割合を体積割合として扱う

            // アルキメデスの原理に基づく浮力の計算
            // 浮力 = 流体の密度 × 沈んでいる部分の体積 × 重力加速度
            // UnityのAddForceでは質量で割る必要がないため、そのまま力として適用
            float effectiveBuoyancyForce = fluidDensity * (objectVolume * submergedVolumeRatio) * Physics.gravity.magnitude;

            // Debug.Log($"Submerged Volume Ratio: {submergedVolumeRatio:F2}, Effective Buoyancy Force: {effectiveBuoyancyForce:F2}");

            // 上向きの力を加える
            rb.AddForce(Vector3.up * effectiveBuoyancyForce, ForceMode.Force);

            Debug.Log(Vector3.up * effectiveBuoyancyForce);
        }
    }
}
