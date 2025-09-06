using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnOnSceneLoad : MonoBehaviour
{
    [Tooltip("シーン開始時に出すプレハブ")]
    public GameObject prefab;

    [Tooltip("スポーンさせる基準オブジェクト（その位置・回転を使う）")]
    public Transform spawnPoint;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (prefab != null && spawnPoint != null)
        {
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("SpawnOnSceneLoad: prefab または spawnPoint が設定されていません。");
        }
    }
}
