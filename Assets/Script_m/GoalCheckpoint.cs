using UnityEngine;

public class GoalCheckpoint : MonoBehaviour
{
    // プレイヤータグを指定
    public string playerTag = "Player";

    // 親オブジェクト（ゴールオブジェクト）のスクリプトへの参照
    private ObjectCollisionCounter parentCounter;

    private void Start()
    {
        // 親オブジェクト（ゴールオブジェクト）の ObjectCollisionCounter スクリプトを取得
        parentCounter = GetComponentInParent<ObjectCollisionCounter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーが接触した場合
        if (other.gameObject.CompareTag(playerTag))
        {
            // チェックポイント通過処理
            if (parentCounter != null)
            {
                parentCounter.PassCheckpoint();  // チェックポイント通過を親オブジェクトに通知
            }
        }
    }
}
