using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    // ParticipantクラスをRankingManagerの内部クラスとして定義
    // これにより、ParticipantはRankingManager.Participantとして参照されますが、
    // ここでは同じファイル内なので単にParticipantと書けます。
    // publicとすることで、他のスクリプトからもこの構造体を認識できますが、
    // 今回の用途ではRankingManager内でのみ使用されるため、privateでも問題ありません。
    public class Participant
    {
        public string Name;           // 参加者の名前（例: "Player", "NPC 1"）
        public int CurrentProgress;   // 現在の目標達成カウント
        public int TargetProgress;    // 目標とする達成カウント
        public bool IsFinished;       // 全ての目標を達成したかどうかのフラグ
        public MonoBehaviour Controller; // 関連付けられたUnityスクリプトへの参照 (ObjectCollisionCounter または NPCGoalManager)
    }

    [Header("参加者設定")]
    [Tooltip("プレイヤーのObjectCollisionCounterスクリプトをここに割り当ててください。")]
    public ObjectCollisionCounter playerCounter;
    [Tooltip("シーン内の全てのNPCGoalManagerスクリプトをここに割り当ててください。")]
    public NPCGoalManager[] npcManagers; // 複数のNPCを管理するための配列

    [Header("UI設定")]
    [Tooltip("プレイヤーのランキング項目が表示されるUIの親Transformをここに割り当ててください。")]
    public Transform rankingUIContainer; // ランキング項目（テキストなど）の親となるUI要素
    [Tooltip("プレイヤーのランキング項目を表示するためのプレハブ（Textコンポーネントを持つGameObject）をここに割り当ててください。")]
    public GameObject rankingEntryPrefab; // 各ランキング項目用のUIプレハブ（例: Text GameObject）

    // プレイヤーのランキング情報を表示するためのテキスト要素を一つだけ管理
    private Text playerRankingUIText;

    private List<Participant> participants = new List<Participant>();

    void Start()
    {
        InitializeParticipants();
        CreateRankingUIElementForPlayer(); // プレイヤー用のUI要素を生成
        UpdateRankingUI(); // 初期表示を更新
    }

    /// <summary>
    /// ゲーム開始時に全ての参加者（プレイヤーとNPC）をリストに登録します。
    /// </summary>
    void InitializeParticipants()
    {
        participants.Clear(); // リストが空であることを確認

        // プレイヤーを参加者リストに追加
        if (playerCounter != null)
        {
            participants.Add(new Participant
            {
                Name = "プレイヤー",
                CurrentProgress = playerCounter.CurrentCount,
                TargetProgress = playerCounter.targetCount,
                IsFinished = playerCounter.GoalReached,
                Controller = playerCounter
            });
        }
        else
        {
            Debug.LogWarning("RankingManager: PlayerCounterが設定されていません。", this);
        }

        // 全てのNPCを参加者リストに追加
        if (npcManagers != null)
        {
            for (int i = 0; i < npcManagers.Length; i++)
            {
                NPCGoalManager npc = npcManagers[i];
                if (npc != null)
                {
                    participants.Add(new Participant
                    {
                        Name = $"NPC {i + 1}",
                        CurrentProgress = npc.GoalPassCount,
                        TargetProgress = npc.goalPassesRequired,
                        IsFinished = npc.IsFinished(),
                        Controller = npc
                    });
                }
            }
        }
        else
        {
            Debug.LogWarning("RankingManager: NPC Managersが設定されていません。", this);
        }

        SortParticipants(); // 最初の順位付け
    }

    /// <summary>
    /// プレイヤーのランキングUI要素を生成します。
    /// 常に1つのUI要素のみを作成し、playerRankingUITextに格納します。
    /// </summary>
    void CreateRankingUIElementForPlayer()
    {
        // 既存のUI要素を全てクリア (念のため)
        foreach (Transform child in rankingUIContainer)
        {
            Destroy(child.gameObject);
        }
        playerRankingUIText = null; // 既存の参照をクリア

        if (rankingEntryPrefab != null)
        {
            GameObject entryObj = Instantiate(rankingEntryPrefab, rankingUIContainer);
            Text entryText = entryObj.GetComponent<Text>();
            if (entryText != null)
            {
                playerRankingUIText = entryText;
                playerRankingUIText.name = "PlayerRankText"; // 管理しやすいように名前を設定
            }
            else
            {
                Debug.LogWarning("Ranking Entry PrefabにTextコンポーネントがありません。", entryObj);
            }
        }
        else
        {
            Debug.LogError("Ranking Entry Prefabが設定されていません。", this);
        }
    }

    /// <summary>
    /// 参加者の進行状況を更新し、プレイヤーのランキングUIを再描画します。
    /// このメソッドは、ObjectCollisionCounterまたはNPCGoalManagerから進行状況が変更された際に呼び出されます。
    /// </summary>
    /// <param name="controller">進行状況を更新するコントローラースクリプト（ObjectCollisionCounterまたはNPCGoalManager）。</param>
    /// <param name="currentProgress">現在の進行カウント。</param>
    /// <param name="isFinished">目標を達成したかどうかのフラグ。</param>
    public void UpdateParticipantProgress(MonoBehaviour controller, int currentProgress, bool isFinished)
    {
        // 該当する参加者を見つけて情報を更新
        Participant participantToUpdate = participants.FirstOrDefault(p => p.Controller == controller);
        if (participantToUpdate != null)
        {
            participantToUpdate.CurrentProgress = currentProgress;
            participantToUpdate.IsFinished = isFinished;
            SortParticipants();   // 順位を再計算
            UpdateRankingUI();    // UIを更新
        }
        else
        {
           
        }
    }

    /// <summary>
    /// 参加者リストを順位に基づいてソートします。
    /// 優先順位: 1. ゴールに到達済み, 2. 現在の進行度が高い順
    /// </summary>
    void SortParticipants()
    {
        participants = participants
            .OrderByDescending(p => p.IsFinished)         // ゴールに到達した参加者が優先
            .ThenByDescending(p => p.CurrentProgress)     // 次に現在の進行度が高い順
            .ToList();
    }

    /// <summary>
    /// 現在の参加者リストに基づいてプレイヤーのランキングUIを更新します。
    /// 表示形式を「X位 (ゴール!)」のように変更します。
    /// </summary>
    void UpdateRankingUI()
    {
        if (playerRankingUIText == null)
        {
            Debug.LogWarning("RankingManager: プレイヤーのランキングUIテキストが設定されていません。");
            return;
        }

        // ソートされたリストからプレイヤーを見つける
        Participant playerParticipant = participants.FirstOrDefault(p => p.Controller == playerCounter);

        if (playerParticipant != null)
        {
            // プレイヤーの順位を計算 (リストのインデックスは0から始まるため+1する)
            int playerRank = participants.IndexOf(playerParticipant) + 1;

            // プレイヤーの順位だけを表示する形式に更新
            playerRankingUIText.text = $"{playerRank}位";
            playerRankingUIText.gameObject.SetActive(true);
        }
        else
        {
            // プレイヤーが見つからない場合、UIを非表示にするか、エラーメッセージを表示する
            playerRankingUIText.text = "プレイヤー情報がありません";
            playerRankingUIText.gameObject.SetActive(false);
            Debug.LogWarning("RankingManager: プレイヤーが参加者リストに見つかりませんでした。");
        }
    }
}