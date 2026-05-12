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
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float standCameraY = 1.6f;
    [SerializeField] private float crouchCameraY = 1.0f;
    [SerializeField] private float cameraSmooth = 10f;

    [Header("점프")]
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float gravity = -9.8f;

    [Header("앉기")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float normalHeight = 2f;

    [Header("시체")]
    [SerializeField] private Transform carryPoint;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private float throwForce = 5f;

    [Header("상호작용에 필요한 시간")]
    [SerializeField] private float holdTime = 2f;

    [Header("애니메이터")]
    public Animator animator;

    [Header("무기 슬롯")]
    public PlayerWeaponSlot playerWeaponSlot;

    private CharacterController characterController;
    private PlayerState state;

    private GameObject carryObject;
    private DeadBodyHighlight currentHighlight;
    private StealthUIManager stealthUIManager;

    private RaycastHit hit;
    private Ray ray;

    private float yVelocity;
    private float xRotation;
    private float cameraY;

    // 입력 캐싱
    private float x;
    private float z;

    private float holdTimer; // 상호작용 누적 시간 / 시체

    private bool canInteract;
    private bool tryCrouch;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        state = GetComponent<PlayerState>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController.height = normalHeight;
        characterController.center = new Vector3(0, normalHeight / 2f, 0);

        cameraY = standCameraY;
        stealthUIManager = StealthUIManager.Instance;
    }

    // 매프레임 실행
    void Update()
    {
        InputCache();   // 입력 캐싱

        StateInput();
        MouseLook();
        Move();
        Gravity();
        Jump();
        Crouch();

        WeaponInput();      // 발사 처리
        WeaponSwapInput();  // 무기 교체

        HighlightBody();

        HoldInteract();
        ThrowBody();
    }

    #region 플레이어 기본 입력 및 상태 처리
    // 입력 (-1 ~ 1)
    private void InputCache()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
    }
    private void StateInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 달리기
            state.SetMoveState(PlayerMoveState.Run);
        }
        else
        {
            // 걷기
            state.SetMoveState(PlayerMoveState.Walk);
        }
    }

    private void MouseLook()
    {
        float sensitivity = 2f;

        float moveX = Input.GetAxis("Mouse X") * sensitivity;
        float moveY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= moveY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * moveX);
    }

    // 이동 처리
    private void Move()
    {
        Vector3 move = transform.right * x + transform.forward * z;
        move = Vector3.ClampMagnitude(move, 1f); // 대각선 이동 속도 고정
        float speed = state.GetSpeed();

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
        // 1. 언제든 입력 받기
        tryCrouch = Input.GetKey(KeyCode.LeftControl);

        float targetCameraY = standCameraY;

        // 2. 실제 속도 적용은 지상일 때
        if (characterController.isGrounded)
        {
            if (tryCrouch)
            {
                characterController.height = crouchHeight;
                // 캐릭터 센터 이동 / 땅에 박힘 금지
                characterController.center = new Vector3(0, crouchHeight / 2f, 0);
                state.SetMoveState(PlayerMoveState.Crouch);
                targetCameraY = crouchCameraY;
            }
            else
            {
                characterController.height = normalHeight;
                // 캐릭터 센터 이동 / 땅에 박힘 금지
                characterController.center = new Vector3(0, normalHeight / 2f, 0);
            }
        }
        else
        {
            // 3. 공중에서는 속도 안 바뀌도록 상태 유지
            if (tryCrouch)
            {
                targetCameraY = crouchCameraY;
            }
            else
            {
                targetCameraY = standCameraY;
            }
        }
        // 카메라 부드럽게 이동
        cameraY = Mathf.Lerp(cameraY, targetCameraY, cameraSmooth * Time.deltaTime);

        Vector3 cameraPos = cameraPivot.localPosition;
        cameraPos.y = cameraY;
        cameraPivot.localPosition = cameraPos;
    }
    #endregion

    #region 무기 입력
    // 발사 입력
    private void WeaponInput()
    {
        if (Input.GetMouseButton(0))
        {
            WeaponController currentWeapon = playerWeaponSlot.GetCurrentWeapon();

            if (currentWeapon != null)
            {
                animator.SetTrigger("Shot");
                currentWeapon.Fire();

                state.SetShot();
            }
        }
    }

    // 무기 교체 입력
    private void WeaponSwapInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        { 
            playerWeaponSlot.EquipSlot1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerWeaponSlot.EquipSlot2();
        }
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

            StealthUIManager.Instance.BlindInteraction();
        }

        ray.origin = cameraPivot.position;
        ray.direction = cameraPivot.forward;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // 시체 태그 확인
            if (hit.collider.transform.root.CompareTag("DeadBody"))
            { 
                DeadBodyHighlight deadBodyHighlight = hit.collider.GetComponentInParent<DeadBodyHighlight>();

                if (deadBodyHighlight != null)
                {
                    // 하이라이트 ON
                    deadBodyHighlight.SetHighlight(true);
                    currentHighlight = deadBodyHighlight;
                    // 인터렉션 ON
                    stealthUIManager.ShowInteraction("[E] 시체 들기");
                }
            }
        }
    }

    // 상호작용 처리 / 상호작용에 걸리는 시간 존재
    private void HoldInteract()
    {
        canInteract = false;

        ray.origin = cameraPivot.position;
        ray.direction = cameraPivot.forward;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.transform.root.CompareTag("DeadBody"))
                canInteract = true;
        }

        if (!canInteract)
        {
            holdTimer = 0f;
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;
            stealthUIManager.ShowInteraction($"들기 진행중... {(holdTimer / holdTime) * 100f:0}%");

            if (holdTimer >= holdTime)
            {
                Interact(hit);
                holdTimer = 0; // 타이머 초기화
            }
        }
        else
        { 
            holdTimer = 0; // 상호작용 실패 및 타이머 초기화
        }
    }

    private void Interact(RaycastHit hit)
    {
        carryObject = hit.collider.transform.root.gameObject;

        Rigidbody rb = carryObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        carryObject.transform.SetParent(carryPoint);
        carryObject.transform.localPosition = Vector3.zero;
        carryObject.transform.localRotation = Quaternion.identity;

        state.SetActionState(PlayerActionState.Carry);
    }

    // 시체 던지기 처리
    private void ThrowBody()
    {
        if (Input.GetKeyDown(KeyCode.F) && carryObject != null)
        {
            GameObject deadBody = carryObject;
            carryObject = null;

            deadBody.transform.SetParent(null);

            Rigidbody rb = deadBody.GetComponent<Rigidbody>();
            if (rb != null)
            { 
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.AddForce(cameraPivot.forward * throwForce, ForceMode.Impulse);
            }
            state.SetActionState(PlayerActionState.None);
        }
    }
    #endregion
}
