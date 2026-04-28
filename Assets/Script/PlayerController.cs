using UnityEngine;

// 플레이어 입력 처리
// 이동 / 점프 / 앉기
// 마우스 시점 처리
// PlayerState를 이용해서 속도 및 상태 관리

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerState))]
public class PlayerController : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] public Transform cameraPivot;  // 카메라 상하 회전용 

    [Header("점프")]
    [SerializeField] public float jumpForce = 1.5f;    // 점프 높이
    [SerializeField] public float gravity = -9.8f;     // 중력 값

    [Header("앉는 높이")]
    [SerializeField] public float crouchHeight = 1f;    // 앉았을때 높이
    [SerializeField] public float normalHeight = 2f;    // 서있을때 높이

    private CharacterController characterController; // 유니티 내장 컴포넌트 사용
    private PlayerState playerState;

    private float yVelocity;
    private float xRotation = 0f;

    // 자기 자신 컴포넌트 캐싱
    void Awake()
    {
        // 같은 오브젝트에서 사용하기위해 Awake에서 캐싱
        characterController = GetComponent<CharacterController>();
        playerState = GetComponent<PlayerState>();
    }

    // 초기 설정
    void Start()
    {
        // 마우스를 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 매프레임 실행
    void Update()
    {
        MouseLook();    // 시점 처리
        Movement();     // 이동 + 점프
        Crouch();       // 앉기 처리
    }

    // 마우스 시점 처리 / 좌우 - 플레이어 회전 / 상하 - 카메라 회전
    private void MouseLook()
    { 
        // 설정 값에서 마우스 감도 가져오기
        float sensitivity = SettingManager.Instance.mouseSensitivity;
        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        // (임시) 상하 회전 제한 (심한 회전을 막기 위함)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        // 카메라 상하 회전 적용
        cameraPivot.localRotation =Quaternion.Euler(xRotation, 0f, 0f);
        // 플레이어 좌우 회전 적용
        transform.Rotate(Vector3.up * mouseX);
    }

    // 이동 + 점프 + 상태 결정
    private void Movement()
    {
        // 입력 (-1 ~ 1)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // 바닥 체크
        bool isGround = characterController.isGrounded;

        // 상태 결정 / PlayerState 사용
        if (isGround)
        {
            if (Input.GetKey(KeyCode.LeftControl)) // L.Ctrl키 - 앉기
            {
                playerState.currentState = PlayerMoveState.Crouch;
            }

            else if (Input.GetKey(KeyCode.LeftShift)) // L.Shift - 달리기
            {
                playerState.currentState = PlayerMoveState.Run;
            }

            else if (x != 0 || z != 0) // x 또는 z 의 입력이 없지 않다면, 걷기
            {
                playerState.currentState = PlayerMoveState.Walk;
            }

            else // 대기
            {
                playerState.currentState = PlayerMoveState.Idle;
            }
        }

        // PlayerState에 따른 속도 가져오기
        float speed = playerState.GetSpeed();
        // 이동 방향 계산 (플레이어 기준)
        Vector3 move = transform.right * x + transform.forward * z;

        // 점프 처리
        if (isGround && yVelocity < 0)
        {
            yVelocity = -2f; // 바닥에 붙이기

            if (Input.GetKeyDown(KeyCode.Space)) // 점프 입력
            {
                // 점프 힘 계산
                yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
                // 상태 변경
                playerState.currentState = PlayerMoveState.Jump;
            }
        }
        // 중력 적용
        yVelocity += gravity * Time.deltaTime;
        // 최종 이동 벡터
        Vector3 velocity = move * speed + Vector3.up * yVelocity;
        // 실제 이동
        characterController.Move(velocity * Time.deltaTime);
    }
    
    // 앉기 처리 (높이 변경)
    private void Crouch()
    {
        if (playerState.currentState == PlayerMoveState.Crouch)
        {
            characterController.height = crouchHeight;
        }
        else
        {
            characterController.height = normalHeight;
        }
    }
}
