using UnityEngine;

/// <summary>
/// NPCが自動で目的地に向かって移動するための制御クラス。
/// VehicleController を継承して、入力値をAIで決定する。
/// </summary>
public class NPCController : VehicleController
{
    [Header("NPC移動設定")]
    [Tooltip("目的地に到達したと見なす距離")]
    public float pointReachThreshold = 3f;

    [Tooltip("この角度以上のターンではドリフトを有効にする")]
    public float driftAngleThreshold = 25f;

    private NPCGoalManager goalManager;
    private Transform currentTarget;

    protected override void Start()
    {
        base.Start();

        // ゴールマネージャーを取得
        goalManager = GetComponent<NPCGoalManager>();
        currentTarget = goalManager.GetNextTarget();
    }

    protected override void Update()
    {
        base.Update();

        // ゴール完了時は停止
        if (goalManager.IsFinished())
        {
            SetInput(0f, false, false, false);
            return;
        }

        if (currentTarget == null) return;

        Vector3 toTarget = currentTarget.position - transform.position;
        float distance = toTarget.magnitude;

        // 目的地に近づいたら次のポイントへ
        if (distance < pointReachThreshold)
        {
            currentTarget = goalManager.GetNextTarget();
            return;
        }

        // ターゲット方向をローカル方向で取得し、旋回角を計算
        Vector3 dirToTarget = toTarget.normalized;
        float angleToTarget = Vector3.SignedAngle(transform.forward, dirToTarget, Vector3.up);

        // 旋回入力（-1〜1に正規化）
        float steer = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        // ドリフトが必要な角度かどうかを判定
        bool shouldDrift = Mathf.Abs(angleToTarget) > driftAngleThreshold;

        // 加速しながら旋回・ドリフト
        SetInput(steer, true, false, shouldDrift);
    }
}
