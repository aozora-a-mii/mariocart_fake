using UnityEngine;

/// <summary>
/// プレイヤーやNPCの車両共通の物理挙動（加速・減速・旋回・ドリフト・ブースト）を処理するクラス。
/// 他のクラスから入力を渡すことで操作される設計。
/// </summary>
public class VehicleController : MonoBehaviour
{
    [Header("速度パラメータ")]
    public float maxSpeed = 30f;                // 通常の最高速度
    public float reverseSpeed = 10f;            // バック時の最高速度
    public float accelerationTime = 3f;         // 最高速度に達するまでの時間
    public float reverseAccelerationTime = 1.5f;  // バック専用の加速時間
    public float brakeForce = 30f;              // 減速時のブレーキ力
    public float turnSpeed = 100f;              // 旋回スピード

    [Header("ドリフト設定")]
    public float driftFactor = 0.95f;           // ドリフト中の横滑り係数
    public float normalFactor = 0.85f;          // 通常時の横滑り係数

    [Header("ブースト設定")]
    public float driftTriggerTime = 2f;         // ブーストが発動するまでのドリフト時間
    public float boostDuration = 3f;            // ブースト持続時間
    public float boostSpeedBonus = 15f;         // ブースト中の追加速度

    private Rigidbody rb;                       // 車両の物理演算を管理する Rigidbody
    private float currentSpeed = 0f;            // 現在の前後方向速度
    private float driftTimer = 0f;              // ドリフトの継続時間
    private bool isBoosting = false;            // 現在ブースト中か
    private float boostTimer = 0f;              // ブースト残り時間
    private float currentMaxSpeed;              // 現在の最高速度（ブースト時含む）
    private bool isDrifting = false;            // 現在ドリフト中か

    // 入力値（外部から SetInput() で設定）
    protected float inputHorizontal = 0f;
    protected bool inputAccel = false;
    protected bool inputBrake = false;
    protected bool inputDrift = false;

    /// <summary>
    /// 初期化処理（Rigidbody の取得、初期値設定）
    /// </summary>
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMaxSpeed = maxSpeed;
    }

    /// <summary>
    /// 毎フレーム、ドリフト・ブーストの管理を行う
    /// </summary>
    protected virtual void Update()
    {
        HandleDriftBoost();
    }

    /// <summary>
    /// FixedUpdate（物理演算）で車両の移動処理を行う
    /// </summary>
    protected virtual void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// ドリフト時間を測り、ブーストを管理する
    /// </summary>
    void HandleDriftBoost()
    {
        isDrifting = inputDrift;

        if (isDrifting)
        {
            driftTimer += Time.deltaTime;

            // ドリフト時間が一定を超えたらブーストを発動
            if (driftTimer >= driftTriggerTime && !isBoosting)
                ActivateBoost();
        }
        else
        {
            driftTimer = 0f;
        }

        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;

            // ブースト時間が終了したらリセット
            if (boostTimer <= 0f)
                EndBoost();
        }
    }

    /// <summary>
    /// 車両の移動・回転・ドリフトを処理する
    /// </summary>
    void HandleMovement()
    {
        bool isReversing = currentSpeed < -0.1f;

        if (inputAccel)
        {
            // 加速処理
            float accelRate = currentMaxSpeed / accelerationTime;
            currentSpeed = Mathf.MoveTowards(currentSpeed, currentMaxSpeed, accelRate * Time.fixedDeltaTime);
        }
        else if (inputBrake)
        {
            if (!isReversing)
            {
                if (Mathf.Abs(currentSpeed) > 0.1f)
                {
                    // ブレーキによる減速
                    currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.fixedDeltaTime);
                }
                else
                {
                    // 停止状態からバックを開始
                    float reverseRate = reverseSpeed / reverseAccelerationTime;
                    currentSpeed = Mathf.MoveTowards(currentSpeed, -reverseSpeed, reverseRate * Time.fixedDeltaTime);
                    Debug.Log("バックします。");
                }
            }
            // バック中はブレーキ入力を無視（自然減速）
        }
        else
        {
            // 入力が無ければ自然減速
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * 0.5f * Time.fixedDeltaTime);
        }

        // ★ 修正版: 重力方向(y)を保持して前後方向の速度を更新
        Vector3 velocity = transform.forward * currentSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        // 一定以上のスピードがあるときだけ回転
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            float turnAmount = inputHorizontal * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        }

        // 横滑り（ドリフト）処理
        ApplyDrift();
    }

    /// <summary>
    /// 横滑り成分をドリフトの有無に応じて制御
    /// </summary>
    void ApplyDrift()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        float factor = isDrifting ? driftFactor : normalFactor;
        localVelocity.x *= factor;
        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }

    /// <summary>
    /// ブースト状態を開始し、最高速度を一時的に上昇
    /// </summary>
    void ActivateBoost()
    {
        isBoosting = true;
        boostTimer = boostDuration;
        currentMaxSpeed = maxSpeed + boostSpeedBonus;
    }

    /// <summary>
    /// ブースト状態を終了し、通常速度に戻す
    /// </summary>
    void EndBoost()
    {
        isBoosting = false;
        currentMaxSpeed = maxSpeed;

        // 現在速度が上限を超えていた場合は下げる
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
    }

    /// <summary>
    /// プレイヤーまたはNPCからの入力を設定するための外部インターフェース
    /// </summary>
    /// <param name="horizontal">旋回（-1〜1）</param>
    /// <param name="accel">加速</param>
    /// <param name="brake">減速・バック</param>
    /// <param name="drift">ドリフト</param>
    public void SetInput(float horizontal, bool accel, bool brake, bool drift)
    {
        inputHorizontal = horizontal;
        inputAccel = accel;
        inputBrake = brake;
        inputDrift = drift;
    }
}
