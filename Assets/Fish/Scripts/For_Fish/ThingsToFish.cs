using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

// 「釣るもの」オブジェクトの共通の処理やフィールドを書いた親クラス
// Prehabのオブジェクトモデルに子クラスをアタッチしてそのモデルのパラメータを各々設定する
// アニメーションはswwimmingとresistの2種類で、wasCaughtフラグのtrue/falseで切り替える
// ゲームが開始したら自動で初期状態はswimmingアニメーションになっている

public abstract class ThingsToFish : MonoBehaviour
{
    public enum MoveState {SWIM, TURN, IDLE, BATTLE, NONE, SHOW}; // 魚の行動状態のステート
    public enum ModelType {FishType1, FishType2, FishType3, FishType4} // 魚のモデルタイプ
    private MoveState moveState = MoveState.IDLE; // 初期設定は待機状態

    private Animator animator; // Animation Controller操作のための参照
    private Rigidbody rb; // Rigidbodyコンポーネントへの参照

    [SerializeField] private RodsController rodsController; // 釣り竿管理クラスへの参照
    [SerializeField] private Texture2D objectTexture; // モデルに張り付ける2Dテクスチャ（画像ファイル）
    [SerializeField] private string objectName; // オブジェクト名
    [SerializeField] private string creator; // オブジェクトの製作者（ID）
    [SerializeField] private int strength; // 体力パラメータ
    [SerializeField] private int power; // 力パラメータ
    [SerializeField] private int weight; // 重量パラメータ（移動速度の設定）
    [SerializeField] private int point; // 得点
    [SerializeField] private float searchDistance = 10f; // 魚が餌を探す範囲
    [SerializeField] private ModelType modelType; // 魚のモデルタイプ

    public string GetObjectName => objectName;
    public string GetCreator => creator;
    public int GetStrength => strength;
    public void SetStrength(int strength) {this.strength = strength;}
    public int GetPower => power;
    public int GetWeight => weight;
    public int GetPoint => point;

    private float speed; // 魚の移動速度（weightから算出する）
    private Vector3 destination; // 移動先
    private BiteScript bite; // 餌の管理クラスへの参照

    // 特定オブジェクトとの接触時処理
    void OnCollisionEnter(Collision collision)
    {
        // 魚が餌オブジェクト（Feed）に当たったら
        if (collision.gameObject.CompareTag("Feed"))
        {
            // 衝突した餌オブジェクトのBiteScriptコンポーネントをbiteに取得する
            if (moveState == MoveState.BATTLE) return; // すでにバトル中なら何もしない
            bite = collision.gameObject.GetComponent<BiteScript>();
            bite.InBattle(this); // 餌側に戦闘開始を通知する
        }

        // それ以外の場合の処理（何にも当たっていない・他の魚に当たるなど）
        else
        {
            if (moveState == MoveState.SWIM)
            {
                moveState = MoveState.IDLE; // IDLE状態に遷移
                rb.AddForce(collision.impulse, ForceMode.Impulse); // 反動を少し与える
            }
        }
    }

    // 泳ぎ中に壁に接触している場合はIDLE状態にする
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && moveState == MoveState.SWIM) moveState = MoveState.IDLE;
    }

    void Awake()
    {
        Init(); // 子クラスで実装する初期化処理を呼び出す
        List<FishTextureData> fishTextures = Config.config.fishTextureDataList.FindAll(data => data.fishModelType == (int)modelType);
        int index = UnityEngine.Random.Range(0, fishTextures.Count + 1);
        if (index < fishTextures.Count)
        {
            StartCoroutine(LoadImage(fishTextures[index]));
        }
        
        name = objectName;
        speed = 1000 / weight; // weightの値を使って移動速度を決定
        destination = transform.position;
        SetNewRandomDestination(); // ランダムな目的地を設定

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        ApplyTextureToModel(); // 設定した2Dテクスチャの貼り付け処理
    }

    public abstract void Init(); // 子クラスで実装する初期化処理

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

    private IEnumerator LoadImage(FishTextureData data)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(new Uri(Path.Combine(Application.persistentDataPath, FishTextureData.fishTextureFolder, data.fishTextureName)).AbsoluteUri);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            objectTexture = DownloadHandlerTexture.GetContent(request);
            name = data.fishName;
            objectName = data.fishName;
            creator = data.fishCreator;
            ApplyTextureToModel();
        }
    }

    // 移動アルゴリズム関連のパラメータ
    private float previousTime;
    private float waitTime = -1;
    private float turnStartTime = -1;

    // 物理演算をする魚の移動のUpdate処理（移動アルゴリズム）
    void FixedUpdate()
    {
        // 魚が向いている方向と目的地方向（ランダムに決定）の角度差を計算
        float angle = Vector3.SignedAngle(destination - transform.position, -transform.right, Vector3.up);
        Vector3 torque = angle / Math.Abs(angle) * Time.fixedDeltaTime * Vector3.down * speed;
        if (float.IsNaN(torque.x) || float.IsNaN(torque.y) || float.IsNaN(torque.z)) torque = Vector3.zero;

        // 毎フレームどの移動状態か確認して処理を変える
        // （スタート→）IDLE → TURN → SWIM → IDLE （ループ）
        // IDLE → BATTLE → (Win → IDLE) or(Lose → SHOW)
        switch (moveState)
        {
            // 決められた目的地（一番近い餌）に向かって泳ぐ
            case MoveState.SWIM:
                rb.AddForce(-transform.right * speed * Time.fixedDeltaTime, ForceMode.Acceleration);
                rb.AddTorque(torque, ForceMode.Acceleration);

                // 目的地との距離が3未満になったら到着判定で再びIDLE状態に遷移させる
                if ((destination - transform.position).magnitude <= 3f) moveState = MoveState.IDLE;
                break;

            // 向きを変えてから泳ぎ出す
            case MoveState.TURN:
                if (turnStartTime == -1) turnStartTime = Time.fixedTime;
                rb.AddTorque(torque, ForceMode.Acceleration);
                if (Math.Abs(angle) <= 30) moveState = MoveState.SWIM; // SWIM状態に遷移
                break;

            // 一定時間待機させてからTURN状態に遷移させる
            case MoveState.IDLE:
                // 待機時間をランダム設定
                if (waitTime == -1)
                {
                    waitTime = UnityEngine.Random.Range(0, 3f);
                    previousTime = Time.fixedTime;
                }

                // 待機が終わったらランダムな目的地を設定して方向転換
                // 魚が時々止まるような自然な動きを実現する
                if (previousTime + waitTime <= Time.fixedTime)
                {
                    moveState = MoveState.TURN;
                    SetNewRandomDestination(); // ランダムな目的地設定
                    waitTime = -1;
                }

                // 餌を探す処理
                float minDistance = float.MaxValue;
                foreach (BiteScript bite in rodsController.bites)
                {
                    float distance = (bite.transform.position - transform.position).magnitude;
                    if (bite.isAbleEat && distance <= searchDistance && minDistance > distance)
                    {
                        // 一番近い餌を目標に設定する
                        minDistance = Mathf.Min(minDistance, distance);
                        SetDestination(bite.transform.position.x, bite.transform.position.z);
                        moveState = MoveState.TURN; // TURN状態に遷移
                        waitTime = -1;
                        break;
                    }
                }
                break;

            // 釣り竿（餌）とのバトル状態
            case MoveState.BATTLE:
                SetDestination(bite.transform.position.x, bite.transform.position.y); // 餌（釣り竿の先）を目的地に設定
                animator.SetBool("wasCaught", true); // resistアニメーションに遷移

                // 釣られて暴れる魚の動きを物理的も表現する
                rb.MovePosition(transform.position + bite.transform.position - transform.TransformPoint(Vector3.right * -2.5f));
                rb.AddForce(-transform.forward * speed * Time.fixedDeltaTime * 2f, ForceMode.Acceleration);
                break;

            // 結果表示用
            case MoveState.SHOW:
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                break;
        }
    }

    // バトル開始処理
    public void InBattle()
    {
        moveState = MoveState.BATTLE; // ステートをBATTLEにする
    }

    // ランダムな目的地設定関数
    public void SetNewRandomDestination()
    {
        destination.x = transform.position.x + UnityEngine.Random.Range(10f, 15f) * (UnityEngine.Random.Range(-1f, 1f) > 0 ? 1 : -1);
        destination.z = transform.position.z + UnityEngine.Random.Range(10f, 15f) * (UnityEngine.Random.Range(-1f, 1f) > 0 ? 1 : -1);
    }

    // 特定座標を目的地に設定する関数
    public void SetDestination(float x, float z)
    {
        destination.x = x;
        destination.z = z;
    }

    // 物理演算以外のUpdate処理
    void Update()
    {
    }

    // 魚が負けた時（プレイヤーが勝った時）に呼ばれる処理
    public virtual void Lose()
    {
        gameObject.SetActive(false); // オブジェクトを非表示化
        moveState = MoveState.SHOW; // SHOW状態に遷移
    }

    // 魚が勝った時（プレイヤーが負けた時）に呼ばれる処理
    public virtual void Win()
    {
        animator.SetBool("wasCaught", false); // swimmingアニメーションに戻す
        moveState = MoveState.IDLE; // IDLE状態に戻す
    }

}