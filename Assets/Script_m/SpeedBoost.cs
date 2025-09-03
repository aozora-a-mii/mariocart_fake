using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    public float boostDuration = 3f;    // スピードブーストの持続時間（秒）
    public float boostSpeed = 50f;      // スピードブースト時の速度
    public float tagTouchTime = 2f;     // タグに触れる時間（秒）

    private PlayerInputController playerInputController; // PlayerInputController参照
    private NPCController npcController;                  // NPCController参照
    private float tagTouchTimer = 0f;    // タグに触れていた時間
    private bool isTouchingTag = false;  // タグに触れているかどうか
    private float originalSpeed;         // 元の最大速度
    private bool isBoosting = false;     // 現在スピードブースト中かどうか

    [Header("SSB Object")]
    public GameObject ssbObject;  // SSBオブジェクトの参照（インスペクターで設定）

    void Start()
    {
        // PlayerInputController または NPCController を取得
        playerInputController = GetComponent<PlayerInputController>();
        npcController = GetComponent<NPCController>();

        if (playerInputController != null)
        {
            originalSpeed = playerInputController.maxSpeed;
        }
        else if (npcController != null)
        {
            originalSpeed = npcController.maxSpeed;
        }
        else
        {
            Debug.LogError("Neither PlayerInputController nor NPCController found on this GameObject.");
            return;
        }

        if (originalSpeed <= 0)
        {
            Debug.LogWarning("Original speed is less than or equal to 0. Check PlayerInputController or NPCController settings.");
        }

        // 最初にSSBオブジェクトを非表示にしておく
        if (ssbObject != null)
        {
            ssbObject.SetActive(false);  // 初期状態では非表示
        }
        else
        {
            Debug.LogWarning("SSB Object is not assigned. Please assign it in the Inspector.");
        }
    }

    void Update()
    {
        // SSタグに触れている場合の処理
        if (isTouchingTag && !isBoosting)
        {
            tagTouchTimer += Time.deltaTime;

            // 2秒間触れていればブースト
            if (tagTouchTimer >= tagTouchTime)
            {
                StartCoroutine(ActivateSpeedBoost());
                tagTouchTimer = 0f;  // タイマーリセット
            }
        }
        else if (!isTouchingTag)
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
            Debug.Log("SS待機");
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
        isBoosting = true;  // ブースト開始中
        Debug.Log("Speed Boost Activated!"); // ブースト開始時のログ
        if (playerInputController != null)
        {
            playerInputController.maxSpeed = boostSpeed;  // 最大速度を変更
        }
        else if (npcController != null)
        {
            npcController.maxSpeed = boostSpeed;  // 最大速度を変更
        }

        // SSBオブジェクトを表示
        if (ssbObject != null)
        {
            ssbObject.SetActive(true);  // ブースト中にSSBオブジェクトを表示
            Debug.Log("SSB Object is now visible.");
        }

        float elapsedTime = 0f;

        // boostDuration秒間スピードブースト
        while (elapsedTime < boostDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ブースト終了後、元の速度に戻す
        if (playerInputController != null)
        {
            playerInputController.maxSpeed = originalSpeed;
        }
        else if (npcController != null)
        {
            npcController.maxSpeed = originalSpeed;
        }

        Debug.Log("Speed Boost Ended! Returning to original speed."); // ブースト終了時のログ

        // SSBオブジェクトを非表示
        if (ssbObject != null)
        {
            ssbObject.SetActive(false);  // ブースト終了時にSSBオブジェクトを非表示
            Debug.Log("SSB Object is now hidden.");
        }

        isBoosting = false;  // ブースト終了
    }
}
