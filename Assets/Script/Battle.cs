using UnityEngine;

// 피격 판정 처리
// 헤드샷 판정
// 최종 데미지 계산
// Target에게 데미지 전달

public class Battle : MonoBehaviour
{
    // 피격 처리 함수
    public void Hit(RaycastHit hit, float damage)
    {
        // 부모 포함 Target 찾기 (Head/Body 분리)
        Target target = hit.collider.GetComponentInParent<Target>();
        // Target이 없으면 종료
        if (target == null) return;

        float finalDamage = damage;
        float headshotDamage = 3f;

        // 헤드샷 판정 (최종데미지 * 헤드샷 배율)
        if (hit.collider.CompareTag("Head"))
        {
            finalDamage *= headshotDamage;
        }

        // 데미지 적용
        target.TakeDamage(finalDamage);
    }

}
