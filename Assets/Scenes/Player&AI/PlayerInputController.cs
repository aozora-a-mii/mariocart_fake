using UnityEngine;

/// <summary>
/// プレイヤーの入力を受け取り、VehicleController に制御入力を渡すクラス。
/// </summary>
public class PlayerInputController : VehicleController
{
    /// <summary>
    /// 毎フレームプレイヤーからの入力を取得し、SetInput に渡す
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // キーボード＆マウスから入力を取得
        float horizontal = Input.GetAxis("Horizontal");           // ←→キーまたはA/Dキー
        bool accel = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);    // ↑キーまたはWキー
        bool brake = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);// ↓キーまたはDキー
        bool drift = Input.GetMouseButton(0);                     // 左クリック（ドリフト）

        // 入力を VehicleController に渡す
        SetInput(horizontal, accel, brake, drift);
    }
}
