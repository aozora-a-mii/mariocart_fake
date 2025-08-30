using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    public float boostDuration = 3f;    // スピードブーストの持続時間（秒）
    public float boostSpeed = 50f;      // スピードブースト時の速度
    public float tagTouchTime = 2f;     // タグに触れる時間（秒）

    private PlayerManager playerManager; // PlayerManager参照
    private float tagTouchTimer = 0f;    // タグに触れていた時間
    private bool isTouchingTag = false;  // タグに触れているかどうか
    private float originalSpeed;         // 元の最大速度

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        originalSpeed = playerManager.maxSpeed;  // 元の速度を保存
    }

    void Update()
    {
        // SSタグに触れている場合の処理
        if (isTouchingTag)
        {
            tagTouchTimer += Time.deltaTime;

            // 2秒間触れていればブースト
            if (tagTouchTimer >= tagTouchTime)
            {
                StartCoroutine(ActivateSpeedBoost());
                tagTouchTimer = 0f;  // タイマーリセット
            }
        }
        else
        {
            // タグから離れたらタイマーをリセット
            tagTouchTimer = 0f;
        }
    }

    // SSタグに触れている時に呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SS"))
        {
            isTouchingTag = true;
        }
    }

    // SSタグから離れた時に呼ばれる
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SS"))
        {
            isTouchingTag = false;
        }
    }

    // スピードブーストを発動させるコルーチン
    private System.Collections.IEnumerator ActivateSpeedBoost()
    {
        playerManager.maxSpeed = boostSpeed;  // 最大速度を変更
        float elapsedTime = 0f;

        // 3秒間スピードブースト
        while (elapsedTime < boostDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 3秒後に元の速度に戻す
        playerManager.maxSpeed = originalSpeed;
    }
}
