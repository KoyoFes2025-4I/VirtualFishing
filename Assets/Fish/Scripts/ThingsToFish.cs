using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BattleManager))]
[RequireComponent(typeof(PointManager))]
[RequireComponent(typeof(InWaterManager))]
[RequireComponent(typeof(Rigidbody))]

// 「釣るもの」の共通の処理やフィールドを書いた親クラス（抽象クラス）

public abstract class ThingsToFish : MonoBehaviour
{

    private Animator animator;
    private BattleManager battleManager;
    private PointManager pointManager;
    private InWaterManager inWaterManager;
    private Rigidbody rb;

    //[SerializeField] private GameObject model; // 3Dモデル
    //[SerializeField] private Texture2D objectTexture; // モデルに張り付ける2Dテクスチャ（UV展開が必要）
    [SerializeField] private string objectName; // オブジェクト名
    [SerializeField] private int strength; // 体力パラメータ
    [SerializeField] private int power; // 力パラメータ
    [SerializeField] private int weight; // 重量（移動速度）パラメータ
    [SerializeField] private int point; // 釣った時の得点
    [SerializeField] private string creater; // 製作者（ID）
    [SerializeField] private string AnimController; // 適用するAnimation Controllerの名前（Resources/Animations配下）

    private float speed; // 魚の移動速度
    private Vector3 directionVector; // 壁に当たった時の魚の反転方向（ランダム）

    private string AnimPath => "Animations/" + AnimController;// Resources以下のパスを作成

    // strengthとpowerのゲッター（バトル処理で使う）
    public int Strength => strength;
    public int Power => power;

    public Vector3 currentPosition { get; private set; } //オブジェクトの現在の3次元座標

    public bool isInWater { get; private set; } = false; // 釣り竿の着水フラグ
    public bool wasCaught { get; private set; } = false; // 餌オブジェクトとの接触フラグ
    public bool hasStartedBattle { get; private set; } = false; // バトル開始のフラグ
    public bool isInBattle { get; private set; } = false; // 魚を釣りあげるバトル中であるフラグ
    public bool wasSuccessFishing { get; private set; } = false; // 釣り上げに成功したフラグ
    public bool wasFinishFishing { get; private set; } = false; // 釣りバトル終了フラグ

    // 各フラグのセッター
    public void SetTrueInBattle() => isInBattle = true;
    public void SetFalseInBattle() => isInBattle = false;
    public void SetSuccessFishing() => wasSuccessFishing = true;
    public void SetFinishFishing() => wasFinishFishing= true;
    public void SetIsInWater() => isInWater = true;

    // 各フラグのゲッター（Animation Controllerで使う）
    public bool IsInWater => isInWater;
    public bool WasCaught => wasCaught;
    public bool IsInBattle => isInBattle;
    public bool WasSuccessFishing => wasSuccessFishing;
    public bool WasFinishFishing => wasFinishFishing;

    // 状態リセット用メソッド
    public void ResetCatchState()
    {
        isInWater = false;
        wasCaught = false;
        hasStartedBattle = false;
        wasSuccessFishing = false;
        wasFinishFishing = false;
    }

    // 特定オブジェクトとの接触時処理
    public void OnCollisionEnter(Collision collision)
    {

        // 魚がFeedオブジェクトに当たったらwasCaughtをTrueにする
        // バトル中はこの接触判定は動作しないようにする
        if (collision.gameObject.CompareTag("Feed") && isInWater && !isInBattle)
        {
            wasCaught = true;
            Debug.Log("魚が釣り上げられました。");
        }

        // 魚オブジェクトが壁か他の魚に当たったら進行方向をランダムに変える（突き抜けないように反射する）
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

    // バトルに勝った時に呼ばれる処理
    protected virtual void WinFishing()
    {

        animator.SetBool("win", true);// winパラメータをtrueにしてアニメーション遷移

        pointManager.AddPoint(point); // 釣り上げた魚の得点を加算
        ResetCatchState(); // フラグの状態のリセット

        Debug.Log($"{creater}作成「{objectName}」を獲得。ポイントは{point}点。");
        
        // 必要なデータはデータベースへ格納するようにしたい

        animator.SetBool("toExit", true);// toExitパラメータをtrueにしてアニメーション遷移

        Destroy(gameObject); // オブジェクトの消去

    }

    // バトルに負けた時に呼ばれる処理
    protected virtual void LoseFishing()
    {

        animator.SetBool("lose", true);// loseパラメータをtrueにしてアニメーション遷移

        ResetCatchState(); // フラグの状態のリセット

        // 負けた時の後の魚オブジェクトに関する処理を書く

        animator.SetBool("toExit", true);// toExitパラメータをtrueにしてアニメーション遷移

        Destroy(gameObject); // オブジェクトの消去

    }
   
    // 魚が捕まってからの処理
    protected virtual void HandleFishing()
    {

        // 着水中に餌オブジェクトと接触した時は次のアニメーションに移行
        if (isInWater && wasCaught)
        {

            animator.SetBool("wasCaught", true);// wasCaughtパラメータをtrueにしてアニメーション遷移

            // バトルが始まる時
            if (!isInBattle && !hasStartedBattle)
            {
                hasStartedBattle = true; // 2度目は呼ばれないようにする
                battleManager.DoBattle(); // バトル処理の開始
            }

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

                Debug.Log($"現在の合計点数は{pointManager.GetSumPoint()}点です。");

            }

        }

    }

    // 依存コンポーネントの初期化処理
    protected virtual void Awake()
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

        pointManager = GetComponent<PointManager>();

        if (pointManager == null)
        {
            pointManager = gameObject.AddComponent<PointManager>();
        }

        inWaterManager = GetComponent<InWaterManager>();

        if (inWaterManager == null)
        {
            inWaterManager = gameObject.AddComponent<InWaterManager>();
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

    protected virtual void Start()
    {
        //ApplyTextureToModel();
    }

    // UV展開済みの3Dモデルに設定した2Dテクスチャを張り付ける処理
    //private void ApplyTextureToModel()
    //{

        // シェーダーを使って新しいマテリアル（3Dモデルの見た目）を作成
        //Material newMaterial = new Material(Shader.Find("Standard"));

        // マテリアルへテクスチャを割り当て
        //newMaterial.mainTexture = objectTexture;

        // 設定した3Dモデル（model）からMeshRendererコンポーネントを取得
        //MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();

        // modelのmeshRendererのマテリアルを新しく作ったマテリアルに差し替える
        //meshRenderer.material = newMaterial;

    //}

    // 物理演算をする魚の移動のUpdate処理
    protected virtual void FixedUpdate()
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
    protected virtual void Update()
    {
        currentPosition = transform.position; // オブジェクトの3次元座標の取得
        HandleFishing(); // 魚が捕まってからの処理

    }

}