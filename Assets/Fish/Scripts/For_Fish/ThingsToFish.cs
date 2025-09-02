using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

// 「釣るもの」の共通の処理やフィールドを書いた親クラス

public abstract class ThingsToFish : MonoBehaviour
{
    public enum MoveState {SWIM, TURN, IDLE, BATTLE, NONE, SHOW};
    private MoveState moveState = MoveState.IDLE;
    private Animator animator;
    private Rigidbody rb;

    [SerializeField] private RodsController rodsController;
    [SerializeField] private Texture2D objectTexture; // モデルに張り付ける2Dテクスチャ（画像ファイル）
    [SerializeField] private string objectName; // オブジェクト名
    [SerializeField] private string creator; // 製作者（ID）
    [SerializeField] private int strength; // 体力パラメータ
    [SerializeField] private int power; // 力パラメータ
    [SerializeField] private int weight; // 重量パラメータ（移動速度の設定）
    [SerializeField] private int point; // 得点
    [SerializeField] private float searchDistance = 10f;

    public string GetObjectName => objectName;
    public string GetCreator => creator;
    public int GetStrength => strength;
    public void SetStrength(int strength) {this.strength = strength;}
    public int GetPower => power;
    public int GetWeight => weight;
    public int GetPoint => point;

    private float speed; // 魚の移動速度
    private Vector3 destination;
    private BiteScript bite;

    // 特定オブジェクトとの接触時処理
    void OnCollisionEnter(Collision collision)
    {
        // 魚が着水中にFeedオブジェクトに当たったら
        if (collision.gameObject.CompareTag("Feed"))
        {
            bite = collision.gameObject.GetComponent<BiteScript>();
            bite.InBattle(this);
        }
        else
        {
            if (moveState == MoveState.SWIM)
            {
                moveState = MoveState.IDLE;
                rb.AddForce(collision.impulse, ForceMode.Impulse);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && moveState == MoveState.SWIM) moveState = MoveState.IDLE;
    }

    void Awake()
    {
        speed = 1000 / weight; // weightの値を使って移動速度を決定
        destination = transform.position;
        SetNewRandomDestination();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        ApplyTextureToModel();
    }

    public abstract void Init();

    // 設定した2Dテクスチャを作ったモデルのUV展開図に張り付ける処理
    private void ApplyTextureToModel()
    {
        // Standardシェーダーを取得
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            Debug.LogError("URPのLitシェーダーが見つかりません。");
            return;
        }

        // シェーダーを使って新しいマテリアルを作成
        Material newMaterial = new Material(shader);

        if (objectTexture == null)
        {
            Debug.LogError("objectTexture が設定されていません。");
            return;
        }

        // マテリアルへ2Dテクスチャ（画像ファイル）を割り当て
        newMaterial.SetTexture("_BaseMap", objectTexture);

        // SkinnedMeshRendererコンポーネントを取得
        SkinnedMeshRenderer meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (meshRenderer != null)
        {

            // modelのmeshRendererのマテリアルを新しく作ったマテリアルに差し替える
            meshRenderer.material = newMaterial;

            // Shadow Atlasに配慮して影設定をOFFにする
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

        }

        else
        {
            Debug.LogError("MeshRenderer が見つかりませんでした。");
        }

    }

    private float previousTime;
    private float waitTime = -1;
    private float turnStartTime = -1;
    // 物理演算をする魚の移動のUpdate処理
    void FixedUpdate()
    {
        float angle = Vector3.SignedAngle(destination - transform.position, -transform.right, Vector3.up);
        Vector3 torque = angle / Math.Abs(angle) * Time.fixedDeltaTime * Vector3.down * speed;
        if (float.IsNaN(torque.x) || float.IsNaN(torque.y) || float.IsNaN(torque.z)) torque = Vector3.zero;

        switch (moveState)
        {
            case MoveState.SWIM:
                rb.AddForce(-transform.right * speed * Time.fixedDeltaTime, ForceMode.Acceleration);
                rb.AddTorque(torque, ForceMode.Acceleration);
                if ((destination - transform.position).magnitude <= 3f) moveState = MoveState.IDLE;
                break;

            case MoveState.TURN:
                if (turnStartTime == -1) turnStartTime = Time.fixedTime;
                rb.AddTorque(torque, ForceMode.Acceleration);
                if (Math.Abs(angle) <= 30) moveState = MoveState.SWIM;
                break;

            case MoveState.IDLE:
                if (waitTime == -1)
                {
                    waitTime = UnityEngine.Random.Range(0, 3f);
                    previousTime = Time.fixedTime;
                }
                if (previousTime + waitTime <= Time.fixedTime)
                {
                    moveState = MoveState.TURN;
                    SetNewRandomDestination();
                    waitTime = -1;
                }

                float minDistance = float.MaxValue;
                foreach (BiteScript bite in rodsController.bites)
                {
                    float distance = (bite.transform.position - transform.position).magnitude;
                    if (bite.isAbleEat && distance <= searchDistance && minDistance > distance)
                    {
                        minDistance = Mathf.Min(minDistance, distance);
                        SetDestination(bite.transform.position.x, bite.transform.position.z);
                        moveState = MoveState.TURN;
                        waitTime = -1;
                        break;
                    }
                }
                break;

            case MoveState.BATTLE:
                SetDestination(bite.transform.position.x, bite.transform.position.y);
                animator.SetBool("wasCaught", true);
                rb.MovePosition(transform.position + bite.transform.position - transform.TransformPoint(Vector3.right * -2.5f));
                rb.AddForce(-transform.forward * speed * Time.fixedDeltaTime * 2f, ForceMode.Acceleration);
                break;

            case MoveState.SHOW:
                break;
        }
    }

    public void InBattle()
    {
        moveState = MoveState.BATTLE;
    }

    public void SetNewRandomDestination()
    {
        destination.x = transform.position.x + UnityEngine.Random.Range(10f, 15f) * (UnityEngine.Random.Range(-1f, 1f) > 0 ? 1 : -1);
        destination.z = transform.position.z + UnityEngine.Random.Range(10f, 15f) * (UnityEngine.Random.Range(-1f, 1f) > 0 ? 1 : -1);
    }

    public void SetDestination(float x, float z)
    {
        destination.x = x;
        destination.z = z;
    }

    // 物理演算以外のUpdate処理
    void Update()
    {
    }

    // 魚が負けた時に呼ばれる処理
    public virtual void Lose()
    {
        gameObject.SetActive(false);
        moveState = MoveState.SHOW;
    }

    // 魚が勝った時に呼ばれる処理
    public virtual void Win()
    {
        animator.SetBool("wasCaught", false);
        moveState = MoveState.IDLE;
    }

}