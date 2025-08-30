using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CourseSelectController : MonoBehaviour
{
    [Header("コース情報パネル")]
    public GameObject[] coursePanels; // 操作したいパネルのGameObjectを設定

    [Header("選択ボタンのImage")]
    public Image[] selectionButtonImages;

    [Header("ボタンの色")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    [Header("遷移先のレースシーン名")]
    public string[] raceSceneNames;

    private int currentCourseIndex = 0;

    void Start()
    {
        // 最初の表示を更新
        UpdateSelection();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentCourseIndex = 1 - currentCourseIndex;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }
    }

    void UpdateSelection()
    {
        // ▼▼▼【ここから変更】▼▼▼
        // SetActiveで表示/非表示を切り替える代わりに、透明度を操作する
        for (int i = 0; i < coursePanels.Length; i++)
        {
            if (coursePanels[i] == null) continue;

            // 選択されているかどうかを判定
            bool isSelected = (i == currentCourseIndex);
            // 選択中なら不透明度100% (1.0f)、非選択なら不透明度20% (0.2f)
            float targetAlpha = isSelected ? 1.0f : 0.5f;

            // パネル自身と、その子オブジェクトに含まれる全てのImageコンポーネントを取得
            Image[] imagesInPanel = coursePanels[i].GetComponentsInChildren<Image>();

            // 取得した全てのImageコンポーネントに対して処理を行う
            foreach (Image img in imagesInPanel)
            {
                // Imageコンポーネントの元の色情報を保持しつつ、アルファ値だけを変更する
                Color currentColor = img.color;
                currentColor.a = targetAlpha;
                img.color = currentColor;
            }
        }
        // ▲▲▲【ここまで変更】▲▲▲


        // ボタンの色の更新ロジックは変更なし
        for (int i = 0; i < selectionButtonImages.Length; i++)
        {
            if (selectionButtonImages[i] != null)
            {
                selectionButtonImages[i].color = (i == currentCourseIndex) ? selectedColor : normalColor;
            }
        }
    }

    void ConfirmSelection()
    {
        // この部分のロジックは変更なし
        if (raceSceneNames.Length > currentCourseIndex && !string.IsNullOrEmpty(raceSceneNames[currentCourseIndex]))
        {
            string sceneToLoad = raceSceneNames[currentCourseIndex];
            Debug.Log(sceneToLoad + " をロードします。");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("遷移先のシーン名が設定されていません。Index: " + currentCourseIndex);
        }
    }
}