using UnityEngine;

// 체력HP 관리
// 데미지 처리
// 사망처리

public class Target : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] private float maxHP = 100f; // 최대 체력

    [Header("사망 설정")]
    [SerializeField] private string deadTag = "DeadBody"; // 사망 태그

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
        if (isDead) return; // 이미 죽었으면 무시
        
        currentHP -= damage; // 체력 감소
        
        if (currentHP <= 0f) { Die(); } // 체력 0이하면 사망
    }

    // 사망 처리
    private void Die()
    {
        isDead = true;
        gameObject.tag = deadTag; // 사망태그로 변경

        DeadBodyHighlight deadBodyHighlight = GetComponent<DeadBodyHighlight>();
        if (deadBodyHighlight != null)
        {
            deadBodyHighlight.OnDeadBody();
        }

        // AI 비활성화
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) { enemyAI.enabled = false; }

        // 이동 비활성화
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null) { characterController.enabled = false; }

        // 애니메이터 비활성화
        Animator animator = GetComponent<Animator>();
        if (animator != null) { animator.enabled = false; }
    }
}
