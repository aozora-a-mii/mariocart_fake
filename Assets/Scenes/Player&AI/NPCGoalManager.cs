using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// NPCが通過するエリアの順序を管理し、ゴール回数をカウントするクラス。
/// </summary>
public class NPCGoalManager : MonoBehaviour
{
    [Tooltip("NPCが進行するエリアの順序")]
    public PathArea[] pathAreas;

    [Tooltip("ゴール（最後のエリア）を通過する必要がある回数")]
    public int goalPassesRequired = 3;

    private int currentAreaIndex = 0;   // 現在進行中のエリアインデックス
    private int goalPassCount = 0;      // ゴール通過回数

    /// <summary>
    /// 次に向かうランダムポイントを取得（次エリアへ進む）
    /// </summary>
    public Transform GetNextTarget()
    {
        if (goalPassCount >= goalPassesRequired)
            return null;

        var area = pathAreas[currentAreaIndex];
        var point = area.GetRandomPoint();

        // エリアのインデックスを1つ進める（ループ）
        currentAreaIndex = (currentAreaIndex + 1) % pathAreas.Length;

        // ゴール（最後のエリア）に到達したかをチェック
        if (currentAreaIndex == 0)
        {
            goalPassCount++;

            if (goalPassCount >= goalPassesRequired)
            {
                Debug.Log($"{name} がゴールに {goalPassesRequired} 回到達しました！");
                // シーンの読み込み
                SceneManager.LoadScene("LOSE");
            }
        }

        return point;
    }

    /// <summary>
    /// ゴール回数に達したか判定
    /// </summary>
    public bool IsFinished()
    {
        return goalPassCount >= goalPassesRequired;
    }
}
