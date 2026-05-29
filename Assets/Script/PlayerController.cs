using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("마우스 민감도")]
    [SerializeField] private float sensitivity = 300f;

    [Header("시체")]
    [SerializeField] private Transform carryPoint;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private float throwForce = 5f;

    [Header("미션 목표 상호 작용")]
    [SerializeField] private float missionTargetHoldTime = 3f;

    [Header("시체 상호작용에 필요한 시간")]
    [SerializeField] private float holdTime = 2f;

    [Header("애니메이터")]
    public Animator animator;

    [Header("무기 슬롯")]
    public PlayerWeaponSlot playerWeaponSlot;

    private CharacterController characterController;
    private PlayerState state;

    private GameObject carryObject;
    private MissionTarget currentMissionTarget;
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

    private float missionTargetHoldTimer;  // 목표물 상호작용 누적 시간 
    private float holdTimer;        // 시체 상호작용 누적 시간

    private bool canInteract;
    private bool tryCrouch;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        state = GetComponent<PlayerState>();
    }

    void Start()
    {
        // 플레이어, 에너미 튕김 무시
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController.height = normalHeight;
        characterController.center = new Vector3(0, normalHeight / 2f, 0);

        cameraY = standCameraY;
        stealthUIManager = StealthUIManager.Instance;

        if (stealthUIManager != null)
        {
            stealthUIManager.UpdateHP(100f, 100f);
        }
    }

    // 매프레임 실행
    void Update()
    {
        if (Time.timeScale == 0f) return; // 결과 UI패널 출력 시 입력 차단

        InputCache();   // 입력 캐싱

        StateInput();
        MouseLook();
        Move();
        Gravity();
        Jump();
        Crouch();

        WeaponInput();      // 발사 처리
        WeaponSwapInput();  // 무기 교체

        HighlightTarget();

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
            state.SetMoveState(PlayerMoveState.Run); // 달리기
        }
        else
        {
            state.SetMoveState(PlayerMoveState.Walk); // 걷기
        }
    }

    private void MouseLook()
    {
        float moveX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

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
        Vector3 velocity = move * speed; // 중력 포함 이동
        velocity.y = yVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    // 점프 처리
    private void Jump()
    {
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
    // 키 입력
    private void WeaponInput()
    {
        WeaponController currentWeapon = playerWeaponSlot.GetCurrentWeapon();

        // 발사
        if (Input.GetMouseButton(0) && currentWeapon != null)
        {
            if (currentWeapon.CanFire())
            {
                animator.SetTrigger("Shot");
                currentWeapon.Fire();
                state.SetShot();
            }
        }

        // 장전
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
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

    // 테스트용 기즈모
    private void OnDrawGizmos()
    {
        if (cameraPivot == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraPivot.position, cameraPivot.forward * interactDistance);
    }

    #region 스텔스모드 전용 - 시체 / 미션 타겟 관련
    // 타겟 하이라이트
    private void HighlightTarget()
    {
        // 이전 하이라이트 제거
        if (currentHighlight != null)
        {
            currentHighlight.SetHighlight(false);
            currentHighlight = null;
        }
        ray.origin = cameraPivot.position;
        ray.direction = cameraPivot.forward;

        if (!Physics.Raycast(ray, out hit, interactDistance))
        {
            stealthUIManager.BlindInteraction(); 
            return;
        }

        // 시체 체크
        DeadBodyHighlight deadBodyHighlight = hit.collider.GetComponentInParent<DeadBodyHighlight>();
        // C4 체크
        MissionTarget c4 = hit.collider.GetComponentInParent<MissionTarget>();

        if (deadBodyHighlight != null)
        {
            // 하이라이트 ON
            deadBodyHighlight.SetHighlight(true);
            currentHighlight = deadBodyHighlight;
            stealthUIManager.ShowInteraction("[E] 시체 들기"); // UI ON
            return;
        }

        if (c4 != null)
        {
            stealthUIManager.ShowInteraction("[E] C4 회수"); // UI ON
            return;
        }
        // 상호작용 범위 아니면 UI OFF
        stealthUIManager.BlindInteraction();
    }

    // 상호작용 처리 / 상호작용에 걸리는 시간 존재
    private void HoldInteract()
    {
        canInteract = false;

        ray.origin = cameraPivot.position;
        ray.direction = cameraPivot.forward;

        if (Physics.Raycast(ray, out hit, interactDistance, ~LayerMask.GetMask("Player")))
        {
            GameObject hitObject = hit.collider.gameObject;

            Debug.Log("히트 => " + hitObject.name);
            Debug.Log("태그 => " + hitObject.tag);

            // C4
            if (hitObject.CompareTag("Mission_C4"))
            {
                canInteract = true;

                if (Input.GetKey(KeyCode.E))
                {
                    // 상호 작용 진행 / 홀드 상태
                    holdTimer += Time.deltaTime;
                    float ratio = Mathf.Clamp01(holdTimer / holdTime);

                    stealthUIManager.ShowMissionHold(); // 상호작용 게이지 UI
                    stealthUIManager.ShowInteraction("C4 회수 중"); // 상호작용 중 텍스트 UI
                    stealthUIManager.UpdateMissionHold(ratio); // 상호작용 중 게이지 증감 UI

                    if (holdTimer >= holdTime)
                    {
                        MissionManager.Instance.CompleteMission();
                        Destroy(hitObject);

                        holdTimer = 0f;
                        stealthUIManager.HideMissionHold(); // 완료 후에도 게이지 숨기기
                    }
                }

                else
                {
                    holdTimer = 0f;
                    stealthUIManager.HideMissionHold(); // E키 떼면 숨기기
                    stealthUIManager.ShowInteraction("[E] C4 회수"); // C4 마우스호버 시
                }
                return;
            }

            else if (hitObject.CompareTag("DeadBody"))
            {
                if (carryObject != null) return; // 이미 시체 들고있으면 리턴

                canInteract = true;

                if (Input.GetKey(KeyCode.E))
                {
                    holdTimer += Time.deltaTime;
                    float ratio = Mathf.Clamp01(holdTimer / holdTime);

                    stealthUIManager.ShowDeadBodyHold(); // 상호작용 게이지 UI
                    stealthUIManager.ShowInteraction("시체 드는 중"); // 상호작용 중 텍스트 UI
                    stealthUIManager.UpdateDeadBodyHold(ratio); // 상호작용 중 게이지 증감 UI

                    if (holdTimer >= holdTime)
                    {
                        Interact(hit);
                        holdTimer = 0f;
                        stealthUIManager.HideDeadBodyHold();
                    }
                }

                else
                {
                    holdTimer = 0f;
                    stealthUIManager.HideDeadBodyHold(); // E키 떼면 숨기기
                    stealthUIManager.ShowInteraction("[E] 시체 들기"); // 시체 마우스호버 시
                }
                return;
            }
        }
        holdTimer = 0f;

        if (stealthUIManager != null)
        {
            stealthUIManager.HideMissionHold();
            stealthUIManager.BlindInteraction();
        }
    }

    private void Interact(RaycastHit hit)
    {
        DeadBodyHighlight deadBodyHighlight = hit.collider.GetComponentInParent<DeadBodyHighlight>();

        if (deadBodyHighlight == null)
        {
            return;
        }

        carryObject = deadBodyHighlight.gameObject;
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

        stealthUIManager.BlindInteraction();
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
