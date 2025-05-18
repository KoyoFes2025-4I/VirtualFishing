using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(FishAnimations))]
[RequireComponent(typeof(BattleManager))]
[RequireComponent(typeof(PointManager))]
// [RequireComponent(typeof(InWaterManager))]

// 「釣るもの」の共通の処理やフィールドを書いた親クラス（抽象クラス）

public abstract class ThingsToFish : MonoBehaviour
{

    // コントローラー班に着水判定を行うプログラムは作ってもらう
    // ゲッターを介してisInWaterフラグで状態を取得する想定
    // 着水時に餌オブジェクトを移動させるための情報も返してもらう→FeedManagerの方で参照

    private FishAnimations fishAnimations;
    private BattleManager battleManager;
    private PointManager pointManager;
    // private InWaterManager inWaterManager;（仮）

    // Inspectorから値のセットが可能なシリアライズフィールド
    [SerializeField] private GameObject model; // UV展開済みの3Dモデル（Blenderで作成）
    [SerializeField] private Texture2D objectTexture; // モデルに張り付ける2Dテクスチャ
    [SerializeField] private string objectName; // オブジェクト名
    [SerializeField] private int strength; //体力パラメータ
    [SerializeField] private int power; //力パラメータ
    [SerializeField] private int weight; //重量（移動速度）パラメータ
    [SerializeField] private int point; //釣った時の得点
    [SerializeField] private string creater; //製作者（ID）

    // strengthとpowerの読み取り用
    public int Strength => strength;
    public int Power => power;

    public Vector3 currentPosition { get; private set; } //オブジェクトの現在の3次元座標

    public bool isInWater { get; private set; } = false; // 釣り竿の着水フラグ
    public bool wasCaught { get; private set; } = false; // 餌オブジェクトとの接触フラグ
    public bool hasStartedBattle { get; private set; } = false; // バトル開始のフラグ
    public bool isInBattle { get; private set; } = false; // 魚を釣りあげるバトル中であるフラグ
    public bool wasSuccessFishing { get; private set; } = false; // 釣り上げに成功したフラグ
    public bool wasFinishFishing { get; private set; } = false; // 釣りバトル終了フラグ

    // 各フラグのセッター用のメソッド（外部ファイルで呼び出す）
    public void SetTrueInBattle() => isInBattle = true;
    public void SetFalseInBattle() => isInBattle = false;
    public void SetSuccessFishing() => wasSuccessFishing = true;
    public void SetFinishFishing() => wasFinishFishing= true;

    // 状態リセット用メソッド
    public void ResetCatchState()
    {
        isInWater = false;
        wasCaught = false;
        hasStartedBattle = false;
        wasSuccessFishing = false;
        wasFinishFishing = false;
    }

    // 餌オブジェクト（タグ名: Feed）と接触した時の処理
    public void OnCollisionEnter(Collision collision)
    {
        // バトル中はこの接触判定は動作しないようにする
        if (collision.gameObject.CompareTag("Feed") && isInWater && !isInBattle)
        {
            wasCaught = true;
        }
    }

    // 魚の泳ぎの動きのアルゴリズムとアニメーション処理
    protected virtual void MovementConfig()
    {

        // 重量のパラメータで移動速度を決めて魚を移動させる
        transform.Translate(Vector3.forward * Time.deltaTime * weight);

        // 壁に当たったら進行方向を変えるようにする（ステージ班と連携）

        fishAnimations.PlayMoveAnimation(); // 魚の泳ぎアニメーションの再生

    }

    // バトルに勝った時に呼ばれる処理
    protected virtual void WinFishing()
    {

        fishAnimations.PlayWinAnimation(); // 成功時のアニメーションの再生

        pointManager.AddPoint(point); // 釣り上げた魚の得点を加算
        ResetCatchState(); // フラグの状態のリセット

        // 釣り上げた魚のモデル、名前、得点などの画面上への表示プロセスを書きたい
        // 必要なデータはデータベースへ格納するようにする

        Destroy(gameObject); // 釣り上げが成功したらそのオブジェクトは消される

    }

    // バトルに負けた時に呼ばれる処理
    protected virtual void LoseFishing()
    {

        fishAnimations.PlayLoseAnimation(); // 失敗時のアニメーションの再生

        ResetCatchState(); // フラグの状態のリセット

        // 負けた時の後の魚オブジェクトに関する処理を書く

    }
   
    // 魚が捕まってからの処理
    protected virtual void HandleFishing()
    {

        // 着水中に餌オブジェクトと接触した時は次のアニメーションに移行
        if (isInWater && wasCaught)
        {

            // バトル中に毎フレーム呼ばれる
            if (isInBattle)
            {
                fishAnimations.PlayCaughtAnimation(); // バトル中の魚が引っかかっているアニメーションの再生
            }

            // バトルが始まる時
            if (!isInBattle && !hasStartedBattle)
            {
                hasStartedBattle = true; // 2度目は呼ばれないようにする
                battleManager.DoBattle(); // バトル処理の開始（中でコルーチンが存在）
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

            }

        }

    }

    // 依存コンポーネントの初期化処理
    protected virtual void Awake()
    {
        fishAnimations = GetComponent<FishAnimations>();
        battleManager = GetComponent<BattleManager>();
        pointManager = GetComponent<PointManager>();
        // inWaterManager = GetComponent<InWaterManager>();
    }

    protected virtual void Start()
    {
        ApplyTextureToModel();
    }

    // UV展開済みの3Dモデルに設定した2Dテクスチャを張り付ける処理
    private void ApplyTextureToModel()
    {

        // シェーダーを使って新しいマテリアル（3Dモデルの見た目）を作成
        Material newMaterial = new Material(Shader.Find("Standard"));

        // マテリアルへテクスチャを割り当て
        newMaterial.mainTexture = objectTexture;

        // 設定した3Dモデル（model）からMeshRendererコンポーネントを取得
        MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();

        // modelのmeshRendererのマテリアルを新しく作ったマテリアルに差し替える
        meshRenderer.material = newMaterial;

    }


    protected virtual void Update()
    {

        // isInWater = inWaterManger.GetIsInWater(); // 着水フラグの状態を取得

        currentPosition = transform.position; //オブジェクトの3次元座標の取得

        if (!wasCaught)
        {
            MovementConfig(); // 毎フレームオブジェクトの位置を動かしながらアニメーション再生
        }

        HandleFishing();

    }

}
    
    
