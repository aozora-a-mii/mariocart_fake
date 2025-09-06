using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager���g�p���邽�߂ɕK�v

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

    // �����L���O�}�l�[�W���[�ւ̎Q��
    // RankingManager�Ƀv���C���[�̐i�s�󋵂�ʒm���邽�߂ɕK�v�ł��B
    public RankingManager rankingManager;

    // RankingManager�����݂̃J�E���g���擾���邽�߂̌��J�v���p�e�B
    public int CurrentCount => currentCount;

    // RankingManager���S�[�����B�t���O���擾���邽�߂̌��J�v���p�e�B
    public bool GoalReached => goalReached;

    void Start()
    {
        // ����Inspector��RankingManager�����蓖�Ă��Ă��Ȃ���΁A�V�[��������T���܂��B
        // �V�[������RankingManager�̃C���X�^���X����������݂��邱�Ƃ�O��Ƃ��Ă��܂��B
        if (rankingManager == null)
        {
            rankingManager = FindObjectOfType<RankingManager>();
            if (rankingManager == null)
            {
                Debug.LogError("RankingManager��������܂���ł����B�V�[����RankingManager�X�N���v�g������GameObject�����邱�Ƃ��m�F���Ă��������B", this);
            }
        }

        // �Q�[���J�n���ɁA���g�̏�����Ԃ�RankingManager�ɒʒm���܂��B
        if (rankingManager != null)
        {
            rankingManager.UpdateParticipantProgress(this, currentCount, goalReached);
        }
    }

    /// <summary>
    /// �S�[���I�u�W�F�N�g�ɐڐG�����ۂɃJ�E���g���A�b�v���ARankingManager�ɒʒm���܂��B
    /// </summary>
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
            checkpointPassed = false; // �S�[����ʉ߂����̂Ń`�F�b�N�|�C���g��Ԃ����Z�b�g

            // �i�s�󋵂��ύX���ꂽ���Ƃ�RankingManager�ɒʒm
            if (rankingManager != null)
            {
                rankingManager.UpdateParticipantProgress(this, currentCount, goalReached);
            }
        }
    }

    /// <summary>
    /// �V�[���J�ڂ����s���܂��B
    /// </summary>
    private void LoadTargetScene()
    {
        // �V�[���̓ǂݍ���
        SceneManager.LoadScene("WIN");
    }

    /// <summary>
    /// �`�F�b�N�|�C���g�ʉߏ����B����S�[�����ɃJ�E���g�A�b�v�ł����Ԃɂ��܂��B
    /// </summary>
    public void PassCheckpoint()
    {
        if (goalReached) return; // �S�[���ɓ��B�ς݂̏ꍇ�͉������Ȃ�

        // �`�F�b�N�|�C���g�ʉ߃t���O�𗧂Ă�
        checkpointPassed = true;
        canIncrementCount = true;  // ����S�[�����ɃJ�E���g�A�b�v�ł���悤�Ƀt���O�𗧂Ă�
        Debug.Log("�`�F�b�N�|�C���g�ʉ߁I");

        // �`�F�b�N�|�C���g�ʉ߂ɂ���ď�Ԃ��ς�������Ƃ�RankingManager�ɒʒm
        // (UI�\���ɉe�������邩������Ȃ�����)
        if (rankingManager != null)
        {
            rankingManager.UpdateParticipantProgress(this, currentCount, goalReached);
        }
    }

    /// <summary>
    /// �g���K�[�R���C�_�[�����I�u�W�F�N�g�Ƃ̐ڐG�����o���܂��B
    /// ���̃X�N���v�g�̓v���C���[�ɃA�^�b�`����Ă��邱�Ƃ�z�肵�Ă��܂��B
    /// </summary>
    /// <param name="other">�ڐG�����R���C�_�[</param>
    private void OnTriggerEnter(Collider other)
    {
        if (goalReached) return; // �S�[���ɓ��B�ς݂̏ꍇ�́A����ȏ�̏����͕s�v�ł��B

        // �����ڐG�����̂��uCheckpoint�v�^�O�����I�u�W�F�N�g�ł����
        if (other.CompareTag("Checkpoint"))
        {
            PassCheckpoint();
        }
        // �����ڐG�����̂��uGoal�v�^�O�����I�u�W�F�N�g�ł����
        else if (other.CompareTag("Goal"))
        {
            // �`�F�b�N�|�C���g��ʉ߂��Ă���ꍇ�̂݃J�E���g�A�b�v
            if (checkpointPassed)
            {
                IncrementCount(); // �J�E���g�A�b�v���������s
                Debug.Log("�S�[���ʉ�!");
            }
            else
            {
                Debug.Log("�S�[���ɓ��B���܂������A�`�F�b�N�|�C���g��ʉ߂��Ă��܂���B");
            }
        }
    }
}