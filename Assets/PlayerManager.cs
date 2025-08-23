using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("速度パラメータ")]
    public float maxSpeed = 30f;        // 通常の最高速度
    public float reverseSpeed = 10f;    // バック最高速度
    public float accelerationTime = 3f; // 最高速到達にかかる時間（秒）
    public float brakeForce = 30f;      // ブレーキ減速力
    public float turnSpeed = 100f;      // ハンドル回転速度

    [Header("ドリフト設定")]
    public float driftFactor = 0.95f;
    public float normalFactor = 0.85f;

    [Header("ブースト設定")]
    public float driftTriggerTime = 2f;
    public float boostDuration = 3f;
    public float boostSpeedBonus = 15f;

    private Rigidbody rb;
    private bool isDrifting = false;
    private float currentSpeed = 0f;
    private float driftTimer = 0f;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private float currentMaxSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMaxSpeed = maxSpeed;
    }

    void Update()
    {
        // 左クリックを押している間だけドリフト
        isDrifting = Input.GetMouseButton(0);

        // ドリフト時間を計測してブースト判定
        if (isDrifting)
        {
            driftTimer += Time.deltaTime;
            if (driftTimer >= driftTriggerTime && !isBoosting)
                ActivateBoost();
        }
        else
        {
            driftTimer = 0f;
        }

        // ブースト管理
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
                EndBoost();
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        bool accelKey = Input.GetKey(KeyCode.UpArrow);
        bool brakeKey = Input.GetKey(KeyCode.DownArrow);

        // 現在バック中か判定
        bool isReversing = currentSpeed < -0.1f;

        // 前進（徐々に加速）
        if (accelKey)
        {
            float accelRate = currentMaxSpeed / accelerationTime;
            currentSpeed = Mathf.MoveTowards(currentSpeed, currentMaxSpeed, accelRate * Time.fixedDeltaTime);
        }
        // ↓キーでの処理
        else if (brakeKey)
        {
            if (!isReversing) // バック中はブレーキを無効化
            {
                if (Mathf.Abs(currentSpeed) > 0.1f)
                {
                    // 減速（前進からのブレーキ）
                    currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.fixedDeltaTime);
                }
                else
                {
                    // 停止中 → バック開始
                    float reverseRate = reverseSpeed / accelerationTime;
                    currentSpeed = Mathf.MoveTowards(currentSpeed, -reverseSpeed, reverseRate * Time.fixedDeltaTime);
                }
            }
            // バック中の場合は何もしない（スロットルを緩めて自然減速のみ）
        }
        else
        {
            // アクセルもブレーキも押していない場合は自然減速
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * 0.5f * Time.fixedDeltaTime);
        }

        // 現在の速度で移動
        rb.linearVelocity = transform.forward * currentSpeed;

        // 回転
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            float turnAmount = horizontal * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        }

        // ドリフト処理
        ApplyDrift();
    }

    void ApplyDrift()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        float factor = isDrifting ? driftFactor : normalFactor;
        localVelocity.x *= factor;
        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }

    void ActivateBoost()
    {
        isBoosting = true;
        boostTimer = boostDuration;
        currentMaxSpeed = maxSpeed + boostSpeedBonus;
    }

    void EndBoost()
    {
        isBoosting = false;
        currentMaxSpeed = maxSpeed;
    }
}
