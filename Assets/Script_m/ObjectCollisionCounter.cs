using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectCollisionCounter : MonoBehaviour
{
    // �ڕW�J�E���g
    public int targetCount = 5; // �C���X�y�N�^�[�Őݒ�ł��܂�

    // ���ۂ̃J�E���g
    private int currentCount = 0;

    // �`�F�b�N�|�C���g�̒ʉ߃t���O
    private bool checkpointPassed = false;

    // ����S�[�����ɃJ�E���g���グ�鏀�����ł��Ă��邩
    private bool canIncrementCount = false;

    // �S�[���ɓ��B�������ǂ���
    private bool goalReached = false;

    // �S�[���I�u�W�F�N�g�ɐڐG�����ۂɃJ�E���g���A�b�v����
    public void IncrementCount()
    {
        if (canIncrementCount && !goalReached)  // ����S�[�����ɃJ�E���g�𑝂₷�t���O�������Ă���ꍇ
        {
            currentCount++;
            Debug.Log("�J�E���g�A�b�v: " + currentCount);

            // �ڕW�J�E���g�ɒB�����ꍇ�A�S�[����F��
            if (currentCount >= targetCount)
            {
                goalReached = true;
                Debug.Log("�ڕW�J�E���g�ɒB���܂���! �S�[��!");
                LoadTargetScene();
            }

            // ����S�[���܂ŃJ�E���g�𑝂₳�Ȃ��悤��
            canIncrementCount = false;
        }
    }

    // �V�[���J��
    private void LoadTargetScene()
    {
        // �V�[���̓ǂݍ���
        SceneManager.LoadScene("WIN");
    }

    // �`�F�b�N�|�C���g�ʉߏ���
    public void PassCheckpoint()
    {
        // �`�F�b�N�|�C���g�ʉ߃t���O�𗧂Ă�
        checkpointPassed = true;
        canIncrementCount = true;  // ����S�[�����ɃJ�E���g�A�b�v�ł���悤�Ƀt���O�𗧂Ă�
        Debug.Log("�`�F�b�N�|�C���g�ʉ߁I");
    }

    // �S�[���ɓ��B���ăJ�E���g�𑝂₷����
    private void OnTriggerEnter(Collider other)
    {
        // �S�[���I�u�W�F�N�g�ɐڐG�����ꍇ
        if (other.gameObject.CompareTag("Player") && checkpointPassed)
        {
            IncrementCount(); // �J�E���g�A�b�v
            checkpointPassed = false;  // �S�[����A�`�F�b�N�|�C���g�����Z�b�g
            Debug.Log("�S�[���ʉ�!");
        }
    }
}
