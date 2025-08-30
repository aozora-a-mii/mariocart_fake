using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagerを使うために必要

public class SceneChanger : MonoBehaviour
{
    // インスペクターからシーン名を指定できるようにpublicなメソッドにする
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // 引数で指定されたシーンをロードする
    }
}