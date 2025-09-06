using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagerを使用するために必要

public class ObjectCollisionCounter : MonoBehaviour
{
    // 目標カウント
    public int targetCount = 5; // インスペクターで設定できます

    // 実際のカウント
    private int currentCount = 0;

    // チェックポイントの通過フラグ
    private bool checkpointPassed = false;

    // 次回ゴール時にカウントを上げる準備ができているか
    private bool canIncrementCount = false;

    // ゴールに到達したかどうか
    private bool goalReached = false;

    // ランキングマネージャーへの参照
    // RankingManagerにプレイヤーの進行状況を通知するために必要です。
    public RankingManager rankingManager;

    // RankingManagerが現在のカウントを取得するための公開プロパティ
    public int CurrentCount => currentCount;

    // RankingManagerがゴール到達フラグを取得するための公開プロパティ
    public bool GoalReached => goalReached;

    void Start()
    {
        // もしInspectorでRankingManagerが割り当てられていなければ、シーン内から探します。
        // シーン内にRankingManagerのインスタンスが一つだけ存在することを前提としています。
        if (rankingManager == null)
        {
            rankingManager = FindObjectOfType<RankingManager>();
            if (rankingManager == null)
            {
                Debug.LogError("RankingManagerが見つかりませんでした。シーンにRankingManagerスクリプトを持つGameObjectがあることを確認してください。", this);
            }
        }

        // ゲーム開始時に、自身の初期状態をRankingManagerに通知します。
        if (rankingManager != null)
        {
            rankingManager.UpdateParticipantProgress(this, currentCount, goalReached);
        }
    }

    /// <summary>
    /// ゴールオブジェクトに接触した際にカウントをアップし、RankingManagerに通知します。
    /// </summary>
    public void IncrementCount()
    {
        if (canIncrementCount && !goalReached)  // 次回ゴール時にカウントを増やすフラグが立っている場合
        {
            currentCount++;
            Debug.Log("カウントアップ: " + currentCount);

            // 目標カウントに達した場合、ゴールを認識
            if (currentCount >= targetCount)
            {
                goalReached = true;
                Debug.Log("目標カウントに達しました! ゴール!");
                LoadTargetScene();
            }

            // 次回ゴールまでカウントを増やさないように
            canIncrementCount = false;
            checkpointPassed = false; // ゴールを通過したのでチェックポイント状態をリセット

            // 進行状況が変更されたことをRankingManagerに通知
            if (rankingManager != null)
            {
                rankingManager.UpdateParticipantProgress(this, currentCount, goalReached);
            }
        }
    }

    /// <summary>
    /// シーン遷移を実行します。
    /// </summary>
    private void LoadTargetScene()
    {
        // シーンの読み込み
        SceneManager.LoadScene("WIN");
    }

    /// <summary>
    /// チェックポイント通過処理。次回ゴール時にカウントアップできる状態にします。
    /// </summary>
    public void PassCheckpoint()
    {
        if (goalReached) return; // ゴールに到達済みの場合は何もしない

        // チェックポイント通過フラグを立てる
        checkpointPassed = true;
        canIncrementCount = true;  // 次回ゴール時にカウントアップできるようにフラグを立てる
        Debug.Log("チェックポイント通過！");

        // チェックポイント通過によって状態が変わったことをRankingManagerに通知
        // (UI表示に影響があるかもしれないため)
        if (rankingManager != null)
        {
            rankingManager.UpdateParticipantProgress(this, currentCount, goalReached);
        }
    }

    /// <summary>
    /// トリガーコライダーを持つオブジェクトとの接触を検出します。
    /// このスクリプトはプレイヤーにアタッチされていることを想定しています。
    /// </summary>
    /// <param name="other">接触したコライダー</param>
    private void OnTriggerEnter(Collider other)
    {
        if (goalReached) return; // ゴールに到達済みの場合は、これ以上の処理は不要です。

        // もし接触したのが「Checkpoint」タグを持つオブジェクトであれば
        if (other.CompareTag("Checkpoint"))
        {
            PassCheckpoint();
        }
        // もし接触したのが「Goal」タグを持つオブジェクトであれば
        else if (other.CompareTag("Goal"))
        {
            // チェックポイントを通過している場合のみカウントアップ
            if (checkpointPassed)
            {
                IncrementCount(); // カウントアップ処理を実行
                Debug.Log("ゴール通過!");
            }
            else
            {
                Debug.Log("ゴールに到達しましたが、チェックポイントを通過していません。");
            }
        }
    }
}