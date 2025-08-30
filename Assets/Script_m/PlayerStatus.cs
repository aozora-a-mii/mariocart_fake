using UnityEngine;

// 上記のItemBoxControllerで定義したインターフェースを実装します
public class PlayerStatus : MonoBehaviour, IPlayerStatus
{
    private int currentRank = 1; // 現在の順位（レースのシステムから更新される想定）
    private ItemType currentItem = ItemType.None; // 現在所持しているアイテム

    // 順位を返すメソッド
    public int GetRank()
    {
        // TODO: ここで実際のレース順位を返す処理を実装してください
        return currentRank;
    }

    // アイテムを持っているかどうかを返すメソッド
    public bool HasItem()
    {
        return currentItem != ItemType.None;
    }

    // アイテムボックスからアイテムを受け取るメソッド
    public void SetItem(ItemType newItem)
    {
        if (!HasItem())
        {
            currentItem = newItem;
            Debug.Log(this.gameObject.name + "がアイテムを取得しました: " + newItem.ToString());
            // TODO: アイテムUIの表示を更新する処理などをここに追加
        }
    }

    // テスト用に順位を変更するコード（実際のゲームでは不要）
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentRank = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentRank = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentRank = 3;

        if (Input.GetKeyDown(KeyCode.Space) && HasItem()) // スペースキーでアイテム使用
        {
            Debug.Log(currentItem.ToString() + " を使用しました！");
            currentItem = ItemType.None;
        }
    }
}