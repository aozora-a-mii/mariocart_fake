using UnityEngine;

public class GoalCheckpoint : MonoBehaviour
{
    // �v���C���[�^�O���w��
    public string playerTag = "Player";

    // �e�I�u�W�F�N�g�i�S�[���I�u�W�F�N�g�j�̃X�N���v�g�ւ̎Q��
    private ObjectCollisionCounter parentCounter;

    private void Start()
    {
        // �e�I�u�W�F�N�g�i�S�[���I�u�W�F�N�g�j�� ObjectCollisionCounter �X�N���v�g���擾
        parentCounter = GetComponentInParent<ObjectCollisionCounter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[���ڐG�����ꍇ
        if (other.gameObject.CompareTag(playerTag))
        {
            // �`�F�b�N�|�C���g�ʉߏ���
            if (parentCounter != null)
            {
                parentCounter.PassCheckpoint();  // �`�F�b�N�|�C���g�ʉ߂�e�I�u�W�F�N�g�ɒʒm
            }
        }
    }
}
