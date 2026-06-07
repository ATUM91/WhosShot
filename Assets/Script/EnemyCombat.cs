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

    private EnemyController enemyController;

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
        enemyController = GetComponent<EnemyController>();
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

    // 현재 탄창
    public int GetCurrentAmmo()
    {
        if (weaponController == null) return 0;
        return weaponController.GetCurrentAmmo();
    }

    public bool IsReload()
    { 
        if (weaponController == null) return false;
        return weaponController.IsReload();
    }

    #region 공격
    public void SetTarget(Transform target)
    {
        if (weaponController == null) return;
        weaponController.SetTarget(target);
    }

    // 공격 처리
    public bool Attack()
    {
        if (weaponController == null) return false;
        if (weaponController.IsReload()) return false;
        if (!weaponController.CanFire()) return false;

        weaponController.Fire(); // 실제 발사
        return true;
    }

    public void StopAttack()
    {
        if (animator == null) return;

        animator.SetBool("IsAttack",false);
    }

    // 공격 자세 유지
    public void KeepAttackAnimation(bool value)
    {
        if (animator == null) return;

        animator.SetBool("IsAttack", value);
    }
    #endregion

    #region 장전
    // 장전 처리
    public bool Reload()
    {
        if (weaponController == null) return false;
        if (weaponController.IsReload()) return false;
        if (!weaponController.Reload()) return false;

        animator.SetBool("IsAttack", false);

        animator.ResetTrigger("Reload");
        animator.SetTrigger("Reload");
        return true;
    }

    // 애니메이션 이벤트
    public void FinishReload()
    {
        if (weaponController == null) return;

        weaponController.FinishReload();
        animator.ResetTrigger("Reload");

        if (enemyController != null)
        { 
            enemyController.OnReload();
        }
    }
    #endregion
}