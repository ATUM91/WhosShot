using UnityEngine;

// 상태 값 관리

public enum PlayerMoveState
{ 
    Walk, Run, Crouch, Jump
}

public enum PlayerPoseState
{
    PistolPose, RiflePose, ShotgunPose
}

public enum PlayerActionState
{
    None, Carry, Throw, Shot
}

public class PlayerState : MonoBehaviour
{
    private PlayerMoveState moveState;
    private PlayerActionState actionState; 

    [Header("무기 기본 포즈")]
    public PlayerPoseState weaponPose = PlayerPoseState.PistolPose;

    [Header("속도")]
    [SerializeField] float walkSpeed = 4f;      // 걷는 이동속도
    [SerializeField] float runSpeed = 7f;       // 뛰는 이동속도
    [SerializeField] float crouchSpeed = 2f;    // 앉은 이동속도
    [SerializeField] float carrySpeed = 2.5f;   // 들고 이동속도

    public float currentSpeed;

    private bool isCarry;

    void Awake()
    {
        moveState = PlayerMoveState.Walk;
        actionState = PlayerActionState.None;
    }

    // 현재 이동 상태
    public PlayerMoveState CurrentState => moveState;
    // 현재 액션 상태
    public PlayerActionState ActionState => actionState;

    // 이동 속도
    public float GetSpeed()
    {
        if (isCarry) return carrySpeed;
        if (moveState == PlayerMoveState.Crouch) return crouchSpeed;
        if (moveState == PlayerMoveState.Run && !isCarry) return runSpeed;

        return walkSpeed;
    }

    // Shot 트리거 함수
    public void SetShot()
    {
        actionState = PlayerActionState.Shot;
    }

    // Shot 리셋 함수
    public void ResetShot()
    {
        if (actionState == PlayerActionState.Shot)
        {
            actionState = PlayerActionState.None;
        }
    }

    // 이동 상태 설정 
    public void SetMoveState(PlayerMoveState state)
    {
        moveState = state;
    }

    // 무기 포즈 설정
    public void SetPoseState(PlayerPoseState state)
    {
        weaponPose = state;
    }

    // 액션 설정 (들기 / 던지기)
    public void SetActionState(PlayerActionState state)
    {
        actionState = state;

        if (state == PlayerActionState.Carry) { isCarry = true; }
        else { isCarry = false; }
    }

    // 최종 애니메이션용 상태 반환
    public object GetFinalState()
    {
        if (actionState == PlayerActionState.Shot) return PlayerActionState.Shot;
        if (actionState == PlayerActionState.Carry) return PlayerActionState.Carry;

        if (moveState == PlayerMoveState.Jump) return PlayerMoveState.Jump;
        if (moveState == PlayerMoveState.Crouch) return PlayerMoveState.Crouch;
        if (moveState == PlayerMoveState.Run) return PlayerMoveState.Run;
        if (moveState == PlayerMoveState.Walk) return PlayerMoveState.Walk;

        return weaponPose; // 아무 입력 없을 때 무기포즈 반환
    }
}
