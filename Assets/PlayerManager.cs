using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("速度パラメータ")]
    public float maxSpeed = 30f;       // 前進の最高速度
    public float reverseSpeed = 10f;   // バック最高速度
    public float accelerationTime = 3f; // 最高速到達にかかる時間（秒）
    public float brakeForce = 30f;     // ブレーキ減速力
    public float turnSpeed = 100f;     // ハンドル回転速度

    [Header("ドリフト設定")]
    public float driftFactor = 0.95f;
    public float normalFactor = 0.85f;

    private Rigidbody rb;
    private bool isDrifting = false;
    private float currentSpeed = 0f; // 前進時の現在速度

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 左クリックでドリフトON/OFF
        if (Input.GetMouseButtonDown(0))
            isDrifting = true;
        if (Input.GetMouseButtonUp(0))
            isDrifting = false;
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        bool accelKey = Input.GetKey(KeyCode.UpArrow);
        bool brakeKey = Input.GetKey(KeyCode.DownArrow);

        // 前進（徐々に加速）
        if (accelKey)
        {
            float accelRate = maxSpeed / accelerationTime; // 1秒あたりの速度増加量
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, accelRate * Time.fixedDeltaTime);
        }
        // ブレーキ or バック
        else if (brakeKey)
        {
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                rb.AddForce(-transform.forward * brakeForce, ForceMode.Acceleration);
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.fixedDeltaTime);
            }
            else
            {
                float reverseRate = reverseSpeed / accelerationTime;
                currentSpeed = Mathf.MoveTowards(currentSpeed, -reverseSpeed, reverseRate * Time.fixedDeltaTime);
            }
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

        // ドリフト
        ApplyDrift();
    }

    void ApplyDrift()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        float factor = isDrifting ? driftFactor : normalFactor;
        localVelocity.x *= factor; // 横滑り減衰
        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }
}