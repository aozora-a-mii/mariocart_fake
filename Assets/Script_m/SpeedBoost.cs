using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    public float boostDuration = 3f;    // �X�s�[�h�u�[�X�g�̎������ԁi�b�j
    public float boostSpeed = 50f;      // �X�s�[�h�u�[�X�g���̑��x
    public float tagTouchTime = 2f;     // �^�O�ɐG��鎞�ԁi�b�j

    private PlayerInputController playerInputController; // PlayerInputController�Q��
    private NPCController npcController;                  // NPCController�Q��
    private float tagTouchTimer = 0f;    // �^�O�ɐG��Ă�������
    private bool isTouchingTag = false;  // �^�O�ɐG��Ă��邩�ǂ���
    private float originalSpeed;         // ���̍ő呬�x
    private bool isBoosting = false;     // ���݃X�s�[�h�u�[�X�g�����ǂ���

    [Header("SSB Object")]
    public GameObject ssbObject;  // SSB�I�u�W�F�N�g�̎Q�Ɓi�C���X�y�N�^�[�Őݒ�j

    void Start()
    {
        // PlayerInputController �܂��� NPCController ���擾
        playerInputController = GetComponent<PlayerInputController>();
        npcController = GetComponent<NPCController>();

        if (playerInputController != null)
        {
            originalSpeed = playerInputController.maxSpeed;
        }
        else if (npcController != null)
        {
            originalSpeed = npcController.maxSpeed;
        }
        else
        {
            Debug.LogError("Neither PlayerInputController nor NPCController found on this GameObject.");
            return;
        }

        if (originalSpeed <= 0)
        {
            Debug.LogWarning("Original speed is less than or equal to 0. Check PlayerInputController or NPCController settings.");
        }

        // �ŏ���SSB�I�u�W�F�N�g���\���ɂ��Ă���
        if (ssbObject != null)
        {
            ssbObject.SetActive(false);  // ������Ԃł͔�\��
        }
        else
        {
            Debug.LogWarning("SSB Object is not assigned. Please assign it in the Inspector.");
        }
    }

    void Update()
    {
        // SS�^�O�ɐG��Ă���ꍇ�̏���
        if (isTouchingTag && !isBoosting)
        {
            tagTouchTimer += Time.deltaTime;

            // 2�b�ԐG��Ă���΃u�[�X�g
            if (tagTouchTimer >= tagTouchTime)
            {
                StartCoroutine(ActivateSpeedBoost());
                tagTouchTimer = 0f;  // �^�C�}�[���Z�b�g
            }
        }
        else if (!isTouchingTag)
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
            Debug.Log("SS�ҋ@");
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
        isBoosting = true;  // �u�[�X�g�J�n��
        Debug.Log("Speed Boost Activated!"); // �u�[�X�g�J�n���̃��O
        if (playerInputController != null)
        {
            playerInputController.maxSpeed = boostSpeed;  // �ő呬�x��ύX
        }
        else if (npcController != null)
        {
            npcController.maxSpeed = boostSpeed;  // �ő呬�x��ύX
        }

        // SSB�I�u�W�F�N�g��\��
        if (ssbObject != null)
        {
            ssbObject.SetActive(true);  // �u�[�X�g����SSB�I�u�W�F�N�g��\��
            Debug.Log("SSB Object is now visible.");
        }

        float elapsedTime = 0f;

        // boostDuration�b�ԃX�s�[�h�u�[�X�g
        while (elapsedTime < boostDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �u�[�X�g�I����A���̑��x�ɖ߂�
        if (playerInputController != null)
        {
            playerInputController.maxSpeed = originalSpeed;
        }
        else if (npcController != null)
        {
            npcController.maxSpeed = originalSpeed;
        }

        Debug.Log("Speed Boost Ended! Returning to original speed."); // �u�[�X�g�I�����̃��O

        // SSB�I�u�W�F�N�g���\��
        if (ssbObject != null)
        {
            ssbObject.SetActive(false);  // �u�[�X�g�I������SSB�I�u�W�F�N�g���\��
            Debug.Log("SSB Object is now hidden.");
        }

        isBoosting = false;  // �u�[�X�g�I��
    }
}
