using UnityEngine;

/// <summary>
/// エリア内の複数の目的地（ポイント）を管理するクラス。
/// </summary>
public class PathArea : MonoBehaviour
{
    [Tooltip("このエリア内に設定する目的地ポイントのリスト")]
    public Transform[] points;

    /// <summary>
    /// ポイントの中からランダムに1つ返す
    /// </summary>
    public Transform GetRandomPoint()
    {
        if (points.Length == 0) return null;
        return points[Random.Range(0, points.Length)];
    }
}
