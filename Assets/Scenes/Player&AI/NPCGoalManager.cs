using UnityEngine;
// using UnityEngine.SceneManagement; // RankingManagerがシーン遷移を管理するため、ここでは不要になります。

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

    // ランキングマネージャーへの参照
    // RankingManagerにNPCの進行状況を通知するために必要です。
    public RankingManager rankingManager;

    // RankingManagerが現在のゴール通過カウントを取得するための公開プロパティ
    public int GoalPassCount => goalPassCount;

    void Start()
    {
        // もしInspectorでRankingManagerが割り当てられていなければ、シーン内から探します。
        // シーン内にRankingManagerのインスタンスが一つだけ存在することを前提としています。
        if (rankingManager == null)
        {
            rankingManager = FindObjectOfType<RankingManager>();
            if (rankingManager == null)
            {
                Debug.LogError("NPCGoalManager: RankingManagerが見つかりませんでした。シーンにRankingManagerスクリプトを持つGameObjectがあることを確認してください。", this);
            }
        }

        // ゲーム開始時に、自身の初期状態をRankingManagerに通知します。
        if (rankingManager != null)
        {
            rankingManager.UpdateParticipantProgress(this, goalPassCount, IsFinished());
        }
    }

    /// <summary>
    /// 次に向かうランダムポイントを取得し、エリアの進行とゴール通過を処理します。
    /// 進行状況が変更された場合、RankingManagerに通知します。
    /// </summary>
    /// <returns>次の目標地点のTransform。ゴール回数に達している場合はnull。</returns>
    public Transform GetNextTarget()
    {
        // ゴール回数に達している場合は、これ以上ターゲットを提供しない
        if (IsFinished())
        {
            return null;
        }

        // 現在のエリアのランダムなポイントを取得
        var area = pathAreas[currentAreaIndex];
        var point = area.GetRandomPoint();

        // エリアのインデックスを1つ進める（配列の最後まで行ったら0に戻る）
        currentAreaIndex = (currentAreaIndex + 1) % pathAreas.Length;

        // ゴール（最後のエリア）を通過し、インデックスが0に戻ったかをチェック
        if (currentAreaIndex == 0) // 例: pathAreas[pathAreas.Length - 1] がゴールで、その次が pathAreas[0] に戻る場合
        {
            goalPassCount++; // ゴール通過回数を増やす
            Debug.Log($"{name} がゴールを {goalPassCount} 回通過しました。目標: {goalPassesRequired}回");

            // 進行状況が変更されたことをRankingManagerに通知
            if (rankingManager != null)
            {
                rankingManager.UpdateParticipantProgress(this, goalPassCount, IsFinished());
            }

            if (IsFinished())
            {
                Debug.Log($"{name} が目標のゴール回数 {goalPassesRequired} 回に到達しました！");
                // NPCがゴールに到達した場合のシーン遷移は、RankingManagerが
                // 全体的なゲームの勝敗を判断した上で行うべきです。
                // ここで直接 "LOSE" シーンに遷移する処理は削除しました。
                // SceneManager.LoadScene("LOSE"); // <-- この行はコメントアウトまたは削除
            }
        }

        return point;
    }

    /// <summary>
    /// 目標とするゴール通過回数に達したかどうかを判定します。
    /// </summary>
    /// <returns>目標回数に達していればtrue、そうでなければfalse。</returns>
    public bool IsFinished()
    {
        return goalPassCount >= goalPassesRequired;
    }
}