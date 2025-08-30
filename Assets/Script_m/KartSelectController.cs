using UnityEngine;
using UnityEngine.UI; // Imageコンポーネントを扱うために必要
using UnityEngine.SceneManagement;

public class KartSelectController : MonoBehaviour
{
    [Header("表示を切り替えるパネル")]
    public GameObject[] kartPanels; // カート1, 2, 3のメインパネル
    public GameObject[] kartPanels2; // メインパネルと連動して切り替わるパネル2

    [Header("選択状態を示すボタン")]
    public Image[] selectionButtonImages; // ボタンのImageコンポーネントを設定

    [Header("ボタンの色")]
    public Color selectedColor = Color.yellow; // 選択中の色
    public Color normalColor = Color.white;    // 通常時の色

    [Header("遷移先のシーン名")]
    public string nextSceneName = "StageSelectScene";

    // PlayerPrefsに保存する際のキー（どのシーンでも同じキーでアクセスする）
    public const string SelectedKartKey = "SelectedKartIndex";

    private int currentKartIndex = 0;

    void Start()
    {
        // 最初に選択されているカート1の状態を反映させる
        UpdateSelection();
    }

    void Update()
    {
        // →矢印キー
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentKartIndex++;
            if (currentKartIndex >= kartPanels.Length)
            {
                currentKartIndex = 0;
            }
            UpdateSelection();
        }

        // ←矢印キー
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentKartIndex--;
            if (currentKartIndex < 0)
            {
                currentKartIndex = kartPanels.Length - 1;
            }
            UpdateSelection();
        }

        // スペースキーで決定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }
    }

    void UpdateSelection()
    {
        // --- 2種類のパネルの表示を更新 ---
        for (int i = 0; i < kartPanels.Length; i++)
        {
            // 現在選択中のインデックスと一致するかどうかで、表示/非表示を切り替える
            bool isActive = (i == currentKartIndex);

            // メインパネルの更新
            if (kartPanels[i] != null)
            {
                kartPanels[i].SetActive(isActive);
            }

            // 連動パネル2の更新 (配列の要素数が同じであることが前提)
            if (kartPanels2.Length > i && kartPanels2[i] != null)
            {
                kartPanels2[i].SetActive(isActive);
            }
        }

        // --- ボタンの色の更新 ---
        for (int i = 0; i < selectionButtonImages.Length; i++)
        {
            if (selectionButtonImages[i] != null)
            {
                // 現在選択中のボタンだけ色を変える
                selectionButtonImages[i].color = (i == currentKartIndex) ? selectedColor : normalColor;
            }
        }
    }

    void ConfirmSelection()
    {
        // --- ★重要：選択したカートの情報を保存 ---
        // PlayerPrefsを使って、選択したカートのインデックス(0, 1, or 2)をデバイスに保存
        PlayerPrefs.SetInt(SelectedKartKey, currentKartIndex);
        PlayerPrefs.Save(); // 念のため保存を確定

        Debug.Log("カート " + (currentKartIndex + 1) + " を選択しました。値を保存します。");

        // 次のシーンへ移動
        SceneManager.LoadScene(nextSceneName);
    }
}