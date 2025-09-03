using UnityEngine;
using UnityEngine.SceneManagement;

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

    // ゴールオブジェクトに接触した際にカウントをアップする
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
        }
    }

    // シーン遷移
    private void LoadTargetScene()
    {
        // シーンの読み込み
        SceneManager.LoadScene("WIN");
    }

    // チェックポイント通過処理
    public void PassCheckpoint()
    {
        // チェックポイント通過フラグを立てる
        checkpointPassed = true;
        canIncrementCount = true;  // 次回ゴール時にカウントアップできるようにフラグを立てる
        Debug.Log("チェックポイント通過！");
    }

    // ゴールに到達してカウントを増やす処理
    private void OnTriggerEnter(Collider other)
    {
        // ゴールオブジェクトに接触した場合
        if (other.gameObject.CompareTag("Player") && checkpointPassed)
        {
            IncrementCount(); // カウントアップ
            checkpointPassed = false;  // ゴール後、チェックポイントをリセット
            Debug.Log("ゴール通過!");
        }
    }
}
