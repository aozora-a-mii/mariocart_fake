using UnityEngine;

public class RainbowColor : MonoBehaviour
{
    private Material material;
    public float speed = 1.0f;

    // 新しい変数
    private bool _isHidden = false;
    private float _hideTimer = 0f;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
    }

    void Update()
    {
        // オブジェクトが非表示状態かどうかをチェック
        if (_isHidden)
        {
            _hideTimer -= Time.deltaTime;

            // 1秒経過したら、オブジェクトを再表示する
            if (_hideTimer <= 0)
            {
                _isHidden = false;
                GetComponent<Renderer>().enabled = true;
                // ここでマテリアルを再度有効にする必要があれば記述
            }
            return; // 非表示中は他の処理（色の変更など）をスキップ
        }

        // 非表示でない場合、通常の色変更処理を実行
        if (material != null)
        {
            float hue = (Time.time * speed) % 1.0f;
            material.color = Color.HSVToRGB(hue, 1.0f, 1.0f);
        }
    }

    // プレイヤーが触れたときに呼ばれるメソッド
    private void OnTriggerEnter(Collider other)
    {
        // 触れたオブジェクトのタグが"Player"であることを確認
        // タグ名が異なる場合はここを変更してください
        if (other.CompareTag("Player"))
        {
            // 非表示状態に設定し、タイマーを開始
            _isHidden = true;
            _hideTimer = 1.0f;
            GetComponent<Renderer>().enabled = false;
        }
    }
}