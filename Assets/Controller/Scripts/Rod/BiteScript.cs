using UnityEngine;

public class BiteScript : MonoBehaviour
{
    public float waterLevel = 0f;         // 水面の高さ
    public float buoyancyForce = 10f;     // 浮力の強さ（調整してください）
    public float radius = 0.5f;           // 球の半径
    public float dragFactor = 2f;
    public bool isAbleEat { get; private set; } = false;
    public bool isBattle { get; private set; } = false;
    [SerializeField]
    RodScript rod;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 position = transform.position;
        float bottom = position.y - radius;

        // 球の底が水面より下にあるなら浮力を与える
        if (bottom < waterLevel)
        {
            float submergedPortion = Mathf.Clamp01((waterLevel - bottom) / (2f * radius));
            float force = buoyancyForce * submergedPortion;
            rb.AddForce(Vector3.up * force, ForceMode.Force);

            rb.AddForce(-rb.linearVelocity * dragFactor * submergedPortion);
        }

        isAbleEat = rb.linearVelocity.magnitude <= 0.1 && gameObject.activeSelf && !isBattle;
    }

    public void InBattle(ThingsToFish thing)
    {
        if (isBattle) return;
        isBattle = true;
        rod.InBattle(thing);
    }

    public void OutBattle()
    {
        isBattle = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
