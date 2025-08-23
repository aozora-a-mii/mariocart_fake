using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("���x�p�����[�^")]
    public float maxSpeed = 30f;        // �ʏ�̍ō����x
    public float reverseSpeed = 10f;    // �o�b�N�ō����x
    public float accelerationTime = 3f; // �ō������B�ɂ����鎞�ԁi�b�j
    public float brakeForce = 30f;      // �u���[�L������
    public float turnSpeed = 100f;      // �n���h����]���x

    [Header("�h���t�g�ݒ�")]
    public float driftFactor = 0.95f;
    public float normalFactor = 0.85f;

    [Header("�u�[�X�g�ݒ�")]
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
        // ���N���b�N�������Ă���Ԃ����h���t�g
        isDrifting = Input.GetMouseButton(0);

        // �h���t�g���Ԃ��v�����ău�[�X�g����
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

        // �u�[�X�g�Ǘ�
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

        // ���݃o�b�N��������
        bool isReversing = currentSpeed < -0.1f;

        // �O�i�i���X�ɉ����j
        if (accelKey)
        {
            float accelRate = currentMaxSpeed / accelerationTime;
            currentSpeed = Mathf.MoveTowards(currentSpeed, currentMaxSpeed, accelRate * Time.fixedDeltaTime);
        }
        // ���L�[�ł̏���
        else if (brakeKey)
        {
            if (!isReversing) // �o�b�N���̓u���[�L�𖳌���
            {
                if (Mathf.Abs(currentSpeed) > 0.1f)
                {
                    // �����i�O�i����̃u���[�L�j
                    currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.fixedDeltaTime);
                }
                else
                {
                    // ��~�� �� �o�b�N�J�n
                    float reverseRate = reverseSpeed / accelerationTime;
                    currentSpeed = Mathf.MoveTowards(currentSpeed, -reverseSpeed, reverseRate * Time.fixedDeltaTime);
                }
            }
            // �o�b�N���̏ꍇ�͉������Ȃ��i�X���b�g�����ɂ߂Ď��R�����̂݁j
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

        // �h���t�g����
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