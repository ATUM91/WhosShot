using UnityEngine;

// 적 사격 처리
// 탄퍼짐 기반 명중률 처리
// WeaponData / DifficultyData 사용

public class EnemyCombat : MonoBehaviour
{
    [Header("무기 데이터")]
    [SerializeField] private WeaponController weaponController;

    [Header("애니메이터")]
    [SerializeField] private Animator animator;

    void Awake()
    {
        if (weaponController == null)
        { 
            weaponController = GetComponentInChildren<WeaponController>();
        }

        if (animator == null)
        { 
            animator = GetComponent<Animator>();
        }
    }

    // 무기 설정
    public void SetWeapon(WeaponData weaponData)
    {
        if (weaponController == null) return;
        weaponController.SetWeaponData(weaponData);
    }

    public void SetWeaponController(WeaponController weapon)
    {
        weaponController = weapon;
    }

    // 공격 가능 거리
    public float GetAttackDistance()
    { 
        if (weaponController == null) return 0f;
        return weaponController.GetWeaponData().range;
    }

    // 공격 처리
    public void Attack()
    {
        if (weaponController == null) return;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        weaponController.Fire();
    }
}