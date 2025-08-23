using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("���x�p�����[�^")]
    public float maxSpeed = 30f;       // �O�i�̍ō����x
    public float reverseSpeed = 10f;   // �o�b�N�ō����x
    public float accelerationTime = 3f; // �ō������B�ɂ����鎞�ԁi�b�j
    public float brakeForce = 30f;     // �u���[�L������
    public float turnSpeed = 100f;     // �n���h����]���x

    [Header("�h���t�g�ݒ�")]
    public float driftFactor = 0.95f;
    public float normalFactor = 0.85f;

    private Rigidbody rb;
    private bool isDrifting = false;
    private float currentSpeed = 0f; // �O�i���̌��ݑ��x

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ���N���b�N�Ńh���t�gON/OFF
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

        // �O�i�i���X�ɉ����j
        if (accelKey)
        {
            float accelRate = maxSpeed / accelerationTime; // 1�b������̑��x������
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, accelRate * Time.fixedDeltaTime);
        }
        // �u���[�L or �o�b�N
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
            // �A�N�Z�����u���[�L�������Ă��Ȃ��ꍇ�͎��R����
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * 0.5f * Time.fixedDeltaTime);
        }

        // ���݂̑��x�ňړ�
        rb.linearVelocity = transform.forward * currentSpeed;

        // ��]
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            float turnAmount = horizontal * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        }

        // �h���t�g
        ApplyDrift();
    }

    void ApplyDrift()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        float factor = isDrifting ? driftFactor : normalFactor;
        localVelocity.x *= factor; // �����茸��
        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }
}