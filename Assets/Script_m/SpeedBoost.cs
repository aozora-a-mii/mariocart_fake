using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    public float boostDuration = 3f;    // �X�s�[�h�u�[�X�g�̎������ԁi�b�j
    public float boostSpeed = 50f;      // �X�s�[�h�u�[�X�g���̑��x
    public float tagTouchTime = 2f;     // �^�O�ɐG��鎞�ԁi�b�j

    private PlayerManager playerManager; // PlayerManager�Q��
    private float tagTouchTimer = 0f;    // �^�O�ɐG��Ă�������
    private bool isTouchingTag = false;  // �^�O�ɐG��Ă��邩�ǂ���
    private float originalSpeed;         // ���̍ő呬�x

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        originalSpeed = playerManager.maxSpeed;  // ���̑��x��ۑ�
    }

    void Update()
    {
        // SS�^�O�ɐG��Ă���ꍇ�̏���
        if (isTouchingTag)
        {
            tagTouchTimer += Time.deltaTime;

            // 2�b�ԐG��Ă���΃u�[�X�g
            if (tagTouchTimer >= tagTouchTime)
            {
                StartCoroutine(ActivateSpeedBoost());
                tagTouchTimer = 0f;  // �^�C�}�[���Z�b�g
            }
        }
        else
        {
            // �^�O���痣�ꂽ��^�C�}�[�����Z�b�g
            tagTouchTimer = 0f;
        }
    }

    // SS�^�O�ɐG��Ă��鎞�ɌĂ΂��
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SS"))
        {
            isTouchingTag = true;
        }
    }

    // SS�^�O���痣�ꂽ���ɌĂ΂��
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SS"))
        {
            isTouchingTag = false;
        }
    }

    // �X�s�[�h�u�[�X�g�𔭓�������R���[�`��
    private System.Collections.IEnumerator ActivateSpeedBoost()
    {
        playerManager.maxSpeed = boostSpeed;  // �ő呬�x��ύX
        float elapsedTime = 0f;

        // 3�b�ԃX�s�[�h�u�[�X�g
        while (elapsedTime < boostDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 3�b��Ɍ��̑��x�ɖ߂�
        playerManager.maxSpeed = originalSpeed;
    }
}
