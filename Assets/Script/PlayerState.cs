using UnityEngine;

// 상태 값 관리

public enum PlayerMoveState
{ 
    Idle, Walk, Run, Crouch, Jump
}

public class PlayerState : MonoBehaviour
{
    public PlayerMoveState currentState;

    [Header("속도")]
    [SerializeField] float walkSpeed = 4f;      // 걷는 이동속도
    [SerializeField] float runSpeed = 7f;       // 뛰는 이동속도
    [SerializeField] float crouchSpeed = 2f;    // 앉은 이동속도

    public float GetSpeed()
    {
        switch (currentState)
        { 
            case PlayerMoveState.Run:
                return runSpeed;
            case PlayerMoveState.Crouch:
                return crouchSpeed;
            case PlayerMoveState.Idle:
            case PlayerMoveState.Walk:
            default:
                return walkSpeed;
        }
    }
}
