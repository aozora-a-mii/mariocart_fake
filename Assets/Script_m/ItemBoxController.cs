using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// アイテムの種類を定義します。Inspectorで見やすいように名前を付けています。
public enum ItemType
{
    None,            // アイテムなし
    Item1_GreenShell, // アイテム1 (緑甲羅)
    Item2_Thunder,    // アイテム2 (サンダー)
    Item3_Banana,     // アイテム3 (バナナの皮)
    Item4_Mushroom,   // アイテム4 (ダッシュキノコ)
    Item5_Star,       // アイテム5 (スター)
    Item6_Blooper     // アイテム6 (ゲッソー)
}

// Inspectorで確率を設定するためのカスタムクラスです。
[System.Serializable]
public class ItemProbability
{
    public ItemType item;
    [Range(0, 100)]
    public float probability; // 確率をパーセントで指定
}

public class ItemBoxController : MonoBehaviour
{
    [Header("1位の時のアイテム確率")]
    public List<ItemProbability> probabilitiesFor1stPlace;

    [Header("2位以下の時のアイテム確率")]
    public List<ItemProbability> probabilitiesForOtherPlaces;

    // アイテムボックスを叩いた後のクールダウン時間（秒）
    [Header("設定")]
    [Tooltip("アイテム取得後、再度有効になるまでの時間")]
    public float respawnTime = 10.0f;

    private void OnTriggerEnter(Collider other)
    {
        // 触れたオブジェクトからキャラクター情報（PlayerStatusなど）を取得
        // ここでは仮に "PlayerStatus" というスクリプト名にしています。
        // ご自身のプロジェクトに合わせてスクリプト名を変更してください。
        var playerStatus = other.GetComponent<IPlayerStatus>();

        // キャラクター情報がない、または既にアイテムを持っている場合は処理を終了
        if (playerStatus == null || playerStatus.HasItem())
        {
            return;
        }

        // 順位に応じて使用する確率リストを決定
        List<ItemProbability> currentProbabilities;
        if (playerStatus.GetRank() == 1)
        {
            currentProbabilities = probabilitiesFor1stPlace;
        }
        else
        {
            currentProbabilities = probabilitiesForOtherPlaces;
        }

        // 確率に基づいてアイテムを選出
        ItemType selectedItem = GetRandomItem(currentProbabilities);

        // 選出されたアイテムをプレイヤーに付与
        if (selectedItem != ItemType.None)
        {
            playerStatus.SetItem(selectedItem);

            // アイテムボックスを一時的に非表示にし、一定時間後に再表示する
            StartCoroutine(RespawnBox());
        }
    }

    // 確率リストからランダムにアイテムを1つ選出するメソッド
    private ItemType GetRandomItem(List<ItemProbability> probabilities)
    {
        float totalProbability = probabilities.Sum(p => p.probability);
        float randomPoint = Random.Range(0, totalProbability);

        foreach (var itemProb in probabilities)
        {
            if (randomPoint < itemProb.probability)
            {
                return itemProb.item;
            }
            else
            {
                randomPoint -= itemProb.probability;
            }
        }
        return ItemType.None; // 万が一選出されなかった場合
    }

    // アイテムボックスを再表示するコルーチン
    private System.Collections.IEnumerator RespawnBox()
    {
        // オブジェクトの見た目と当たり判定を無効化
        // 見た目（MeshRenderer）と当たり判定（Collider）を無効にするのが一般的
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // 指定した時間待つ
        yield return new WaitForSeconds(respawnTime);

        // 再び有効化する
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }
}


// キャラクターが持つべき情報のインターフェース（例）
// キャラクターのスクリプトにこのインターフェースを実装してください。
public interface IPlayerStatus
{
    int GetRank();          // 順位を返す
    bool HasItem();         // アイテムを持っているか返す
    void SetItem(ItemType item); // アイテムをセットする
}