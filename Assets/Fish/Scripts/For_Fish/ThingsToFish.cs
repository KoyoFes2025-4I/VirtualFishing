using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

// 「釣るもの」の共通の処理やフィールドを書いた親クラス

public abstract class ThingsToFish : MonoBehaviour
{
    public enum MoveState {SWIM, TURN, IDLE};
    public MoveState moveState = MoveState.IDLE;
    private Animator animator;
    private Rigidbody rb;

    [SerializeField] private Texture2D objectTexture; // モデルに張り付ける2Dテクスチャ（画像ファイル）
    [SerializeField] private string objectName; // オブジェクト名
    [SerializeField] private string creater; // 製作者（ID）
    [SerializeField] private int strength; // 体力パラメータ
    [SerializeField] private int power; // 力パラメータ
    [SerializeField] private int weight; // 重量パラメータ（移動速度の設定）
    [SerializeField] private int point; // 得点

    private float speed; // 魚の移動速度
    public Vector3 destination;

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
        SetNewRandomDestination();
        rb = GetComponent<Rigidbody>();
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
                break;
        }
    }

    public void SetNewRandomDestination()
    {
        destination.x = transform.position.x + UnityEngine.Random.Range(10f, 15f) * (UnityEngine.Random.Range(-1f, 1f) > 0 ? 1 : -1);
        destination.z = transform.position.z + UnityEngine.Random.Range(10f, 15f) * (UnityEngine.Random.Range(-1f, 1f) > 0 ? 1 : -1);
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
            animator.SetBool("wasCaught", true);// wasCaughtパラメータをtrueにしてバトル中のアニメーションへ遷移

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
                ResetCatchState(); // フラグの状態のリセット
                Destroy(gameObject); // オブジェクトの消去（勝っても負けても）

            }

        }

    }

    // バトルに勝った時に呼ばれる処理
    protected virtual void WinFishing()
    {

        animator.SetBool("win", true);// winパラメータをtrueにして勝利アニメーションへ遷移

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
        wasCaught = false;
        wasSuccessFishing = false;
        wasFinishFishing = false;
    }

}