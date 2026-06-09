using Cinemachine.Utility;
using UnityEngine;

// 플레이어 감지
// 시체 감지
// 시야각 / 거리 / 벽 체크 담당

public class EnemyVision : MonoBehaviour
{
    [Header("플레이어 감지")]
    [SerializeField] public float viewDistance = 15f;
    [SerializeField] private float viewAngle = 180f;

    [Header("시체 감지")]
    [SerializeField] private float deadBodyCheckDistance = 10f;
    [SerializeField] private LayerMask deadBodyMask;

    [Header("감지 끊김 방지")]
    [SerializeField] private float memoryTime = 0.3f;

    [Header("장애물")]
    [SerializeField] private LayerMask obstacleMask;

    private Transform player;
    private float lastSeenTimer;

    void Start()
    {
        // 플레이어 찾기
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    #region 플레이어 감지
    // 플레이어 시야 체크
    public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 target = player.position + Vector3.up * 1.0f;
        
        return CanSeeTarget(target, viewDistance);
    }
    #endregion

    #region 시체 감지
    // 시체 감지
    public bool CanSeeDeadBody(out Vector3 deadBodyPosition)
    {
        deadBodyPosition = Vector3.zero;

        // 범위 내 시체 검색
        Collider[] deadBody = Physics.OverlapSphere(transform.position, deadBodyCheckDistance, deadBodyMask);

        // 시체 없음
        if (deadBody == null || deadBody.Length == 0) return false;

        float closestDistance = Mathf.Infinity;
        Transform closestBody = null;

        int i = 0;

        while (i < deadBody.Length)
        {
            DeadBodyHighlight body = deadBody[i].GetComponentInParent<DeadBodyHighlight>();

            if (body == null)
            {
                i++;
                continue;
            }

            // 시체 상태가 아닌건 제외
            if (!body.IsDeadBody())
            {
                i++;
                continue;
            }

            float distance = Vector3.Distance(transform.position, deadBody[i].transform.position);

            // 가장 가까운 시체 저장
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBody = deadBody[i].transform;
            }
            i++;
        }
        // 최종 시체 없음
        if (closestBody == null) return false;

        if (!CanSeeTarget(closestBody.position, deadBodyCheckDistance)) return false;

        // 시체 위치 반환
        deadBodyPosition = closestBody.position;
        return true;
    }
    #endregion

    #region 시야각/벽 체크 (중복 간소화)
    private bool CanSeeTarget(Vector3 target, float maxDistance)
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 toTarget = target - origin;

        if (toTarget.magnitude > maxDistance) return false;
        
        // 시야각 체크
        float angle = Vector3.Angle(transform.forward, toTarget);
        if (angle > viewAngle * 0.5f) return false;

        // 벽 체크
        if (Physics.Raycast(origin, toTarget.normalized, toTarget.magnitude, obstacleMask)) return false;

        return true;
    }
    #endregion
}