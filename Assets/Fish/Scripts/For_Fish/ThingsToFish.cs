using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BattleManager))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

// 「釣るもの」の共通の処理やフィールドを書いた親クラス

public abstract class ThingsToFish : MonoBehaviour
{

    private Animator animator;
    private BattleManager battleManager;
    private Rigidbody rb;

    // 衝突したFeedオブジェクトのコンポーネントへの参照を保持しておくための変数
    private InBattleManager currentFeedBattleManager;
    private PointManager currentFeedPointManager;
    private InWaterManager currentFeedInWaterManager;

    [SerializeField] private Texture2D objectTexture; // モデルに張り付ける2Dテクスチャ（画像ファイル）
    [SerializeField] private string AnimController; // 適用するAnimation Controllerの名前（Resources/Animations配下）
    [SerializeField] private string objectName; // オブジェクト名
    [SerializeField] private string creater; // 製作者（ID）
    [SerializeField] private int strength; // 体力パラメータ
    [SerializeField] private int power; // 力パラメータ
    [SerializeField] private int weight; // 重量パラメータ（移動速度の設定）
    [SerializeField] private int point; // 得点

    private float speed; // 魚の移動速度
    private Vector3 directionVector; // 壁に当たった時の魚の反転方向（ランダムに決まる）

    private string AnimPath => "Animations/" + AnimController;// Resources以下のパスを作成

    public bool wasCaught { get; private set; } = false; // 餌オブジェクトとの接触フラグ
    public bool wasSuccessFishing { get; private set; } = false; // 釣り上げに成功したフラグ
    public bool wasFinishFishing { get; private set; } = false; // 釣りバトル終了フラグ

    // strengthとpowerのゲッター（バトル処理で使う）
    public int Strength => strength;
    public int Power => power;

    // セッター
    public void SetSuccessFishing() => wasSuccessFishing = true;
    public void SetFinishFishing() => wasFinishFishing= true;

    // 特定オブジェクトとの接触時処理
    void OnCollisionEnter(Collision collision)
    {

        // 魚が着水中にFeedオブジェクトに当たったら
        if (collision.gameObject.CompareTag("Feed"))
        {
            Debug.Log("Feedに当たりました");
            // 衝突した餌オブジェクトのInWaterManagerを取得
            currentFeedInWaterManager = collision.gameObject.GetComponent<InWaterManager>();

            // その餌オブジェクトが着水中ならば
            if (currentFeedInWaterManager != null && currentFeedInWaterManager.GetInWater())
            {
                // 衝突した餌オブジェクトのInBattleManagerを取得しつつ保持しておく
                currentFeedBattleManager = collision.gameObject.GetComponent<InBattleManager>();

                // その餌オブジェクトが既にバトル中でなければwasCaughtをtrueにする
                if (currentFeedBattleManager != null && !currentFeedBattleManager.GetInBattle())
                {
                    wasCaught = true; // 魚が捕まったフラグ
                    Debug.Log("魚が釣り上げられました。");
                    currentFeedPointManager = collision.gameObject.GetComponent<PointManager>(); // 衝突した餌オブジェクトのPointManagerを保持しておく
                }
            }
        }

        // 魚オブジェクトが壁や他の魚に当たったら進行方向をランダムに変える（突き抜けないように反射する）
        else
        {
            // 壁の法線を取得
            Vector3 normal = collision.contacts[0].normal;

            // 現在の進行方向を壁法線で反射
            directionVector = Vector3.Reflect(directionVector, normal);

            // 少しランダムな揺らぎを加える（XZ平面のみ）
            directionVector += new Vector3(
                Random.Range(-0.2f, 0.2f),
                0f,
                Random.Range(-0.2f, 0.2f)
            );

            directionVector = directionVector.normalized;
        }

    }
   
    void Awake()
    {

        speed = 1000000 / weight; // weightの値を使って移動速度を決定

        // -1.0 〜 1.0 の範囲で魚の動く方向をランダムに決める
        directionVector = new Vector3(
            Random.Range(-1f, 1f),
            0f, // Y軸方向には動かさない（水平だけ）
            Random.Range(-1f, 1f)
        ).normalized;

        battleManager = GetComponent<BattleManager>();

        // 継承した場合に全ての子クラスでも自動でコンポーネントを取得させる
        if (battleManager == null)
        {
            battleManager = gameObject.AddComponent<BattleManager>();
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"{gameObject.name} にAnimatorコンポーネントがありません");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {

            // Assets/Resources/Animations/FishAnimationLogic.controllerを読み込む
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(AnimPath);

            // 各オブジェクトのアニメーション遷移はAnimator Controllerで制御する
            if (controller != null)
            {
                animator.runtimeAnimatorController = controller;　// 指定したAnimator Controllerを自動アタッチ
            }

            else
            {
                Debug.LogWarning(AnimPath + "は見つかりませんでした");
            }

        }

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.linearDamping = 1.0f; // 水中の抵抗感
        rb.useGravity = false; // 重力を無効化
        rb.constraints = RigidbodyConstraints.FreezePositionY; // Y軸方向の動きを固定
        rb.constraints = RigidbodyConstraints.FreezePositionY; // Y軸方向の回転を固定
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Detectionをcoutinuousにする

    }

    void Start()
    {
        ApplyTextureToModel();
    }

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

    // 物理演算をする魚の移動のUpdate処理
    void FixedUpdate()
    {
        if (!wasCaught)
        {
            // 毎フレーム魚を移動させる
            rb.AddForce(directionVector * speed * Time.fixedDeltaTime, ForceMode.Acceleration);
            // 進行方向に向きを合わせる（進行方向に合わせて回転）
            transform.right = -directionVector;
        }
        else
        {
            // 魚が捕まったら移動しないようにする
            rb.linearVelocity = Vector3.zero;
        }
    }

    // 物理演算以外のUpdate処理
    void Update()
    {
        HandleFishing();

    }

    // 魚が捕まってからの処理
    void HandleFishing()
    {
        if (wasCaught)
        {

            currentFeedBattleManager.SetTrueInBattle(); // バトル中フラグをtrueにしておく
            animator.SetBool("wasCaught", true);// wasCaughtパラメータをtrueにしてバトル中のアニメーションへ遷移

            battleManager.DoBattle(); // バトル処理の開始

            // バトル処理が終了した時
            if (wasFinishFishing)
            {

                // 魚の釣り上げに成功していた時
                if (wasSuccessFishing)
                {
                    WinFishing();
                }

                // 魚の釣り上げに失敗していた時
                else
                {
                    LoseFishing();
                }

                Debug.Log($"{currentFeedPointManager.GetPlayerID()}さんの現在の合計点数は{currentFeedPointManager.GetSumPoint()}点です。");

                currentFeedBattleManager.SetFalseInBattle(); // バトル中フラグをfalseにしておく
                ResetCatchState(); // フラグの状態のリセット
                Destroy(gameObject); // オブジェクトの消去（勝っても負けても）

            }

        }

    }

    // バトルに勝った時に呼ばれる処理
    protected virtual void WinFishing()
    {

        animator.SetBool("win", true);// winパラメータをtrueにして勝利アニメーションへ遷移

        currentFeedPointManager.AddPoint(point); // 釣り上げた魚の得点を加算

        Debug.Log("勝ちました");
        Debug.Log($"{creater}作成「{objectName}」を獲得。ポイントは{point}点。");

        // 必要なデータはデータベースへ格納するようにしたい

        animator.SetBool("toExit", true);// toExitパラメータをtrueにしてアニメーション遷移

    }

    // バトルに負けた時に呼ばれる処理
    protected virtual void LoseFishing()
    {

        animator.SetBool("lose", true);// loseパラメータをtrueにして敗北アニメーションへ遷移

        // 負けた時の後の魚オブジェクトに関する処理を書く
        Debug.Log("負けました");

        animator.SetBool("toExit", true);// toExitパラメータをtrueにしてアニメーション遷移

    }

    // 各フラグの状態リセット
    void ResetCatchState()
    {
        currentFeedInWaterManager.SetFalseInWater(); // 一通りの処理が終わったら着水状態は解除させる
        wasCaught = false;
        wasSuccessFishing = false;
        wasFinishFishing = false;
        currentFeedBattleManager = null;
        currentFeedPointManager = null;
        currentFeedInWaterManager = null;
    }

}