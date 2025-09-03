using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// NPC���ʉ߂���G���A�̏������Ǘ����A�S�[���񐔂��J�E���g����N���X�B
/// </summary>
public class NPCGoalManager : MonoBehaviour
{
    [Tooltip("NPC���i�s����G���A�̏���")]
    public PathArea[] pathAreas;

    [Tooltip("�S�[���i�Ō�̃G���A�j��ʉ߂���K�v�������")]
    public int goalPassesRequired = 3;

    private int currentAreaIndex = 0;   // ���ݐi�s���̃G���A�C���f�b�N�X
    private int goalPassCount = 0;      // �S�[���ʉ߉�

    /// <summary>
    /// ���Ɍ����������_���|�C���g���擾�i���G���A�֐i�ށj
    /// </summary>
    public Transform GetNextTarget()
    {
        if (goalPassCount >= goalPassesRequired)
            return null;

        var area = pathAreas[currentAreaIndex];
        var point = area.GetRandomPoint();

        // �G���A�̃C���f�b�N�X��1�i�߂�i���[�v�j
        currentAreaIndex = (currentAreaIndex + 1) % pathAreas.Length;

        // �S�[���i�Ō�̃G���A�j�ɓ��B���������`�F�b�N
        if (currentAreaIndex == 0)
        {
            goalPassCount++;

            if (goalPassCount >= goalPassesRequired)
            {
                Debug.Log($"{name} ���S�[���� {goalPassesRequired} �񓞒B���܂����I");
                // �V�[���̓ǂݍ���
                SceneManager.LoadScene("LOSE");
            }
        }

        return point;
    }

    /// <summary>
    /// �S�[���񐔂ɒB����������
    /// </summary>
    public bool IsFinished()
    {
        return goalPassCount >= goalPassesRequired;
    }
}
