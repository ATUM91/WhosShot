using UnityEngine;

// 체력HP 관리
// 데미지 처리
// 사망처리

public class Target : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] private float maxHP = 100f; // 최대 체력

    [Header("사망 설정")]
    [SerializeField] private string deadBodyTag = "DeadBody";   // 시체 태그
    [SerializeField] private int deadBodyLayer; // 시체 레이어

    [Header("래그돌 콜라이더")]
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject body;

    [SerializeField] private GameObject arm_L_up;
    [SerializeField] private GameObject arm_R_up;
    [SerializeField] private GameObject arm_L_low;
    [SerializeField] private GameObject arm_R_low;
    
    [SerializeField] private GameObject leg_L_up;
    [SerializeField] private GameObject leg_R_up;
    [SerializeField] private GameObject leg_L_low;
    [SerializeField] private GameObject leg_R_low;

    private float currentHP; // 현재 체력
    private bool isDead; // 사망 여부

    // 캐싱
    private EnemyController enemyController;
    private PlayerController playerController;
    private CharacterController characterController;
    private DeadBodyHighlight deadBodyHighlight;

    void Awake()
    {
        // 게임 시작 시 체력 초기화
        currentHP = maxHP;

        enemyController = GetComponent<EnemyController>();
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        deadBodyHighlight = GetComponent<DeadBodyHighlight>();

        Debug.Log("===== TARGET INFO =====");
        Debug.Log("Root : " + gameObject.name);

        if (head != null)
        {
            Debug.Log("Head Pos : " + head.transform.position);
        }

        if (body != null)
        {
            Debug.Log("Body Pos : " + body.transform.position);
        }
    }

    public float CheckDamage(string hitTag, float baseDamage)
    {
        if (hitTag == "Head") return baseDamage * 5;
        if (hitTag == "Body") return baseDamage;
        if (hitTag == "Arm") return baseDamage * 0.7f;
        if (hitTag == "Leg") return baseDamage * 0.5f;

        return baseDamage;
    }

    // 데미지 처리
    public void TakeDamage(float damage)
    {
        Debug.Log($"피격됨 HP: {currentHP}");
        if (isDead) return; // 이미 죽었으면 무시
        
        currentHP -= damage; // 체력 감소
        
        if (currentHP <= 0f) 
        {
            // 체력 0이하면 사망
            Die(); 
        } 
    }

    // 사망 처리
    private void Die()
    {
        isDead = true;

        // 사망상태로 변경 전 저장
        bool isPlayer = CompareTag("Player");

        gameObject.tag = deadBodyTag;
        gameObject.layer = deadBodyLayer;

        // 전체 레이어 통일
        Transform[] all = GetComponentsInChildren<Transform>();
        int i = 0;
        while (i < all.Length)
        {
            all[i].gameObject.layer = deadBodyLayer;
            i++;
        }

        // ragdoll 활성화
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        i = 0;
        while (i < rbs.Length)
        {
            rbs[i].isKinematic = false;
            rbs[i].useGravity = true;
            rbs[i].velocity = Vector3.zero;
            rbs[i].angularVelocity = Vector3.zero;
            i++;
        }

        // 플레이어 사망
        if (isPlayer)
        {
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            // 플레이어 이동 비활성화
            if (characterController != null)
            {
                characterController.enabled = false;
            }
            MissionManager.Instance?.SetPlayerDead();
        }

        // 적 사망
        else
        {
            // 적 AI 사망상태로 전환
            if (enemyController != null)
            {
                enemyController.OnDead();
            }

            // 적 이동 비활성화
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            if (deadBodyHighlight != null)
            {
                deadBodyHighlight.OnDeadBody();
            }
            MissionUI.Instance?.AddKill();
        }
    } 
}
