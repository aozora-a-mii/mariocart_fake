using UnityEngine;

/// <summary>
/// �v���C���[�̓��͂��󂯎��AVehicleController �ɐ�����͂�n���N���X�B
/// </summary>
public class PlayerInputController : VehicleController
{
    /// <summary>
    /// ���t���[���v���C���[����̓��͂��擾���ASetInput �ɓn��
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // �L�[�{�[�h���}�E�X������͂��擾
        float horizontal = Input.GetAxis("Horizontal");           // �����L�[�܂���A/D�L�[
        bool accel = Input.GetKey(KeyCode.UpArrow);               // ���L�[
        bool brake = Input.GetKey(KeyCode.DownArrow);             // ���L�[
        bool drift = Input.GetMouseButton(0);                     // ���N���b�N�i�h���t�g�j

        // ���͂� VehicleController �ɓn��
        SetInput(horizontal, accel, brake, drift);
    }
}
