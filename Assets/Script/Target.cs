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

    private float currentHP; // 현재 체력
    private bool isDead; // 사망 여부

    void Awake()
    {
        // 게임 시작 시 체력 초기화
        currentHP = maxHP;
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
        
        gameObject.tag = deadBodyTag;
        gameObject.layer = deadBodyLayer;

        transform.Find("HeadShot Bound").tag = deadBodyTag;
        transform.Find("HeadShot Bound").gameObject.layer = deadBodyLayer;

        transform.Find("BodyHit Bound").tag = deadBodyTag;
        transform.Find("BodyHit Bound").gameObject.layer = deadBodyLayer;

        DeadBodyHighlight deadBodyHighlight = GetComponent<DeadBodyHighlight>();
        if (deadBodyHighlight != null)
        {
            deadBodyHighlight.OnDeadBody();
        }

        // 적 AI 사망상태로 전환
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null) 
        { 
            enemyController.OnDead();
        }
        
        // 이동 비활성화
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null) 
        { 
            characterController.enabled = false; 
        }
        MissionUI.Instance.AddKill();
    }
}
