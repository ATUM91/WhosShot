using UnityEngine;

// 체력HP 관리
// 데미지 처리
// 사망처리

public class Target : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] private float maxHP = 100f; // 최대 체력

    private float currentHP; // 현재 체력

    void Awake()
    {
        // 게임 시작 시 체력 초기화
        currentHP = maxHP;
    }

    // 데미지 처리
    public void TakeDamage(float damage)
    { 
        // 체력 감소
        currentHP -= damage;
        // 체력 0이하면 사망
        if (currentHP <= 0f)
        {
            Die();
        }
    }

    // 사망 처리
    private void Die()
    {
        // 현재는 단순 삭제 이후 CharacterManager에서 시체 생성 / 리스폰 처리
        Destroy(gameObject);
    }
}
