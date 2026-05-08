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
    [SerializeField] public Transform cameraPivot;          // 카메라 상하 회전용 
    [SerializeField] private float standCameraY = 1.6f;     // 서있을 때 카메라 높이
    [SerializeField] private float crouchCameraY = 1.0f;    // 앉았을 때 카메라 높이
    [SerializeField] private float cameraSmooth = 10f;      // 카메라 이동 속도

    [Header("점프")]
    [SerializeField] public float jumpForce = 1.5f;    // 점프 높이
    [SerializeField] public float gravity = -9.8f;     // 중력 값

    [Header("앉는 높이")]
    [SerializeField] public float crouchHeight = 1f;    // 앉았을때 높이
    [SerializeField] public float normalHeight = 2f;    // 서있을때 높이

    [Header("시체")]
    [SerializeField] private Transform carryPoint;          // 들고 있을 시체 위치
    [SerializeField] private float interactDistance = 3f;   // 상호작용 거리
    [SerializeField] private float throwDistance = 5f;      // 던지는 거리

    [Header("애니메이터")]
    [SerializeField] private Animator animator;

    private CharacterController characterController; // 유니티 내장 컴포넌트 사용
    private DeadBodyHighlight currentHighlight;
    private PlayerState playerState;
    private GameObject carryObject;  // 들고 있는 오브젝트

    private float currentCameraY;
    private float yVelocity;
    private float xRotation;

    // 입력 캐싱
    private float x;
    private float z;

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
        Cursor.visible = false;

        // 초기 카메라 높이 저장
        currentCameraY = standCameraY;
    }

    // 매프레임 실행
    void Update()
    {
        InputCache();   // 입력 캐싱
        MouseLook();    // 시점
        Gravity();      // 중력 계산
        Jump();         // 점프
        State();        // 상태 결정
        Move();         // 이동
        Crouch();       // 앉기

        HighlightBody();// 시체 하이라이트
        Interact();     // 시체 들기
        Throw();        // 던지기

        animator.SetFloat("Speed", characterController.velocity.magnitude);
        animator.SetBool("IsGrounded", characterController.isGrounded);
    }

    #region 플레이어 기본 입력 및 상태 처리
    // 입력 (-1 ~ 1)
    private void InputCache()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
    }

    // 마우스 시점 처리 / 좌우 - 플레이어 회전 / 상하 - 카메라 회전
    private void MouseLook()
    {
        if (SettingManager.Instance == null) return;

        // 설정 값에서 마우스 감도 가져오기
        float sensitivity = SettingManager.Instance.mouseSensitivity;

        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // (임시) 상하 회전 제한 (심한 회전을 막기 위함)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // 카메라 상하 회전 적용
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // 플레이어 좌우 회전 적용
        transform.Rotate(Vector3.up * mouseX);
    }

    // 이동 + 점프 + 상태 결정
    private void State()
    {
        // 공중이면 점프 상태 유지
        if (!characterController.isGrounded)
        {
            playerState.currentState = PlayerMoveState.Jump;
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl)) // L.Ctrl키 - 앉기
        {
            playerState.currentState = PlayerMoveState.Crouch;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && z > 0) // L.Shift - 달리기
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

    // 이동 처리
    private void Move()
    {
        Vector3 move = transform.right * x + transform.forward * z;
        move = Vector3.ClampMagnitude(move, 1f); // 대각선 이동 속도 고정
        float speed = playerState.GetSpeed();

        // 중력 포함 이동
        Vector3 velocity = move * speed;
        velocity.y = yVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    // 점프 처리
    private void Jump()
    {
        // 점프 처리
        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // 점프 힘 계산
            yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    // 중력 처리
    private void Gravity()
    {
        if (characterController.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f; // 바닥에 붙이기
        }
        yVelocity += gravity * Time.deltaTime;
    }

    // 앉기 처리 (높이 변경)
    private void Crouch()
    {
        float targetCameraY = standCameraY;

        // 앉기 상태
        if (playerState.currentState == PlayerMoveState.Crouch)
        {
            characterController.height = crouchHeight;
            // 캐릭터 센터 이동 / 땅에 박힘 금지
            characterController.center = new Vector3(0, crouchHeight / 2f, 0);
            targetCameraY = crouchCameraY;
        }
        else
        {
            characterController.height = normalHeight;
            // 캐릭터 센터 이동 / 땅에 박힘 금지
            characterController.center = new Vector3(0, normalHeight / 2f, 0);
        }

        // 카메라 부드럽게 이동
        currentCameraY = Mathf.Lerp(currentCameraY, targetCameraY, cameraSmooth * Time.deltaTime);

        Vector3 cameraPos = cameraPivot.localPosition;
        cameraPos.y = currentCameraY;
        cameraPivot.localPosition = cameraPos;
    }
    #endregion

    #region 스텔스모드 전용 - 시체 관련
    // 시체 하이라이트
    private void HighlightBody()
    {
        // 이전 하이라이트 제거
        if (currentHighlight != null)
        {
            currentHighlight.SetHighlight(false);
            currentHighlight = null;
        }

        // 화면 중앙에서 레이 발사
        Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // 시체 태그 확인
            if (hit.collider.transform.root.CompareTag("DeadBody"))
            { 
                DeadBodyHighlight deadBodyHighlight = hit.collider.GetComponentInParent<DeadBodyHighlight>();
                if (deadBodyHighlight != null)
                { 
                    deadBodyHighlight.SetHighlight(true);
                    currentHighlight = deadBodyHighlight;
                }
            }
        }
    }

    // 시체 들기 처리
    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        { 
            // 시선 방향 레이
            Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.transform.root.CompareTag("DeadBody"))
                {
                    PickupBody(hit.collider.transform.root.gameObject);
                }
            }
        }
    }

    // 시체 들기
    private void PickupBody(GameObject gameObject)
    {
        if (carryObject != null) return;
        carryObject = gameObject;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        gameObject.transform.SetParent(carryPoint, false);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;

        animator.SetTrigger("Take");
    }

    // 시체 던지기
    private void Throw()
    {
        if (Input.GetKeyDown(KeyCode.F) && carryObject != null)
        {
            GameObject gameObject = carryObject;
            carryObject = null;
            gameObject.transform.SetParent(null);

            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            { 
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddForce(cameraPivot.forward * throwDistance, ForceMode.Impulse);
            }
        }
    }
    #endregion
}
