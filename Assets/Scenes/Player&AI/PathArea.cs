using UnityEngine;

/// <summary>
/// �G���A���̕����̖ړI�n�i�|�C���g�j���Ǘ�����N���X�B
/// </summary>
public class PathArea : MonoBehaviour
{
    [Tooltip("���̃G���A���ɐݒ肷��ړI�n�|�C���g�̃��X�g")]
    public Transform[] points;

    /// <summary>
    /// �|�C���g�̒����烉���_����1�Ԃ�
    /// </summary>
    public Transform GetRandomPoint()
    {
        if (points.Length == 0) return null;
        return points[Random.Range(0, points.Length)];
    }
}
