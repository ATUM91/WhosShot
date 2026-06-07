using UnityEngine;
using UnityEngine.AI;

// 적 AI 메인 상태머신
// 공용 / 스텔스 / 팀데스매치 로직 분리

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(EnemyCombat))]
public class EnemyController : MonoBehaviour
{
    // 적 AI 상태 관리
    public enum EnemyState
    {
        Patrol,     // 순찰 루트 이동
        Idle,       // 도착 후 잠시 정지
        Alert,      // 확정 단계 / 적 머리 위 (!)출력
        Search,     // 수색
        Chase,      // 추적
        Attack,     // 공격
        Reload,     // 장전
        Return,     // 복귀
        Dead,       // 사망
        DeadBody    // 시체
    }

    private enum AlertTagetType
    { 
        None, Player, DeadBody
    }

    [Header("현재 상태")]
    [SerializeField] private EnemyState currentState;
    [SerializeField] private AlertTagetType alertTagetType;
    public EnemyState state => currentState;

    [Header("게임 모드")]
    [SerializeField] private ModeData modeData;

    [Header("이동 속도")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;

    [Header("무기")]
    [SerializeField] private WeaponData[] weaponList;   // 무기목록
    [SerializeField] private Transform weaponPoint;     // 무기 장착 위치

    #region 스텔스 전용 설정
    [Header("대기")]
    [SerializeField] private float idleTime = 3f;

    [Header("경계")]
    [SerializeField] private float alertTime = 2f;

    [Header("수색")]
    [SerializeField] private float searchTime = 7f;
    [SerializeField] private float searchRange = 5f;

    [Header("의심 / 경계 UI")]
    [SerializeField] private GameObject headSuspectIcon;
    [SerializeField] private GameObject headAlertIcon;

    [Header("발각 게이지")]
    [SerializeField] private float detectMax = 100f;        // 게이지 최대치
    [SerializeField] private float detectSpeed = 100f;       // 게이지 올라가는 속도
    [SerializeField] private float detectLoseSpeed = 20f;   // 게이지 내려가는 속도
    #endregion

    private WeaponController weaponController;
    private EnemyVision enemyVision;
    private EnemyCombat enemyCombat;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Transform player;
    private Transform[] patrolPoint;

    private int patrolIndex;

    private float detectGauge;

    private float idleTimer;
    private float alertTimer;
    private float searchTimer;
    private float deadTimer;

    private bool patrolForward = true;
    private bool alertDecided;

    private Vector3 lastSeenPosition;
    private Vector3 startPosition;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyVision = GetComponent<EnemyVision>();
        enemyCombat = GetComponent<EnemyCombat>();
        animator = GetComponent<Animator>();
        
        navMeshAgent.updateRotation = true;
    }

    void Start()
    {
        // 시작 위치 저장
        startPosition = transform.position;
        // 무기 장착
        EquipRandomWeapon();

        // 플레이어 찾기
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            enemyCombat.SetTarget(player);
        }
        // 시작 상태
        ChangeState(EnemyState.Patrol);
        Debug.Log(navMeshAgent.isOnNavMesh);
    }

    void Update()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.DeadBody)
        {
            UpdateAnimation();
            UpdateStateUI();
            return;
        }

        if (currentState == EnemyState.Attack)
        {
            UpdateAttack();
            UpdateAnimation();
            UpdateStateUI();
            return;
        }

        if (currentState == EnemyState.Reload)
        {
            UpdateReload();
            UpdateAnimation();
            UpdateStateUI();
            return;
        }
        // 상태 업데이트
        UpdatePerception();
        UpdateState();
        UpdateAnimation();
        UpdateStateUI();
    }

    #region 각종 업데이트 / 상태변경
    // 공통 업데이트
    // 랜덤 무기 장착
    private void EquipRandomWeapon()
    {
        if (weaponList == null || weaponList.Length == 0) return;

        int randomIndex = Random.Range(0, weaponList.Length);
        WeaponData weapon = weaponList[randomIndex];

        GameObject weaponObject = null;

        if (weapon.weaponPrefab != null)
        {
            weaponObject = Instantiate(weapon.weaponPrefab, weaponPoint.position, weaponPoint.rotation, weaponPoint);
            enemyCombat.SetWeaponController(weaponObject.GetComponent<WeaponController>());
        }
        enemyCombat.SetWeapon(weapon);

        if (weaponObject != null)
        {
            WeaponIK weaponIK = GetComponent<WeaponIK>();
            Transform leftTarget = weaponObject.transform.Find("LeftHandTarget");

            if (weaponIK != null && leftTarget != null)
            { 
                weaponIK.SetLeftHandTarget(leftTarget);
            }
        }
        weaponController = GetComponentInChildren<WeaponController>();

        if (weaponController == null)
        {
            Debug.Log("웨폰 컨트롤러 없음");
        }
    }

    // 감지 센서 역할
    private void UpdatePerception()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.DeadBody) return;

        bool seePlayer = enemyVision.CanSeePlayer();
        bool seeBody = enemyVision.CanSeeDeadBody(out Vector3 bodyPos);

        // 2f 거리내로 근접하면 즉시 발각
        if (seePlayer)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= 2f)
            {
                detectGauge = detectMax;
            }
        }

        // 플레이어 발견 시
        if (seePlayer)
        {
            lastSeenPosition = player.position;

            float distance = Vector3.Distance(transform.position, player.position);
            float close = 1f - Mathf.Clamp01(distance / enemyVision.viewDistance); // 거리 비율 (가까움(1) ~ 멈(0))
            close *= close; // 게이지 상승속도 올리기

            detectGauge += detectSpeed * close * Time.deltaTime;    // 거리 기반 게이지 증가
        }

        else
        {
            // 발각되었으면 게이지 상태 유지
            bool isDetect =
                currentState == EnemyState.Alert ||
                currentState == EnemyState.Chase ||
                currentState == EnemyState.Attack ||
                currentState == EnemyState.Reload ||
                currentState == EnemyState.Search;

            if (!isDetect)
            {
                // 아무것도 안보일 경우 게이지 감소
                detectGauge -= detectLoseSpeed * Time.deltaTime;
            }
        }
        detectGauge = Mathf.Clamp(detectGauge, 0f, detectMax);
        
        // 시체 발견 시
        if (seeBody)
        {
            lastSeenPosition = bodyPos;
            detectGauge = detectMax;
            alertTagetType = AlertTagetType.DeadBody;
        }

        if (detectGauge >= detectMax &&
            currentState != EnemyState.Alert &&
            currentState != EnemyState.Chase &&
            currentState != EnemyState.Attack &&
            currentState != EnemyState.Reload)
        {
            ChangeState(EnemyState.Alert);
        }
        DetectManager.Instance.UpdateDetect(detectGauge);
        //Debug.Log("DetectGauge: " + detectGauge);
    }
    

    // 애니메이션 업데이트
    private void UpdateAnimation()
    {

        float moveSpeed = 0f;
        // 0 = 정지 / 0.5 = 걷기 / 1 = 달리기
        switch (currentState)
        {
            // 걷기
            case EnemyState.Patrol:   
                moveSpeed = 0.5f;
                break;

            // 달리기
            case EnemyState.Chase: 
            case EnemyState.Search: 
                moveSpeed = 1f; 
                break;

            // 정지
            case EnemyState.Idle:
            case EnemyState.Alert:
            case EnemyState.Attack:
            case EnemyState.Reload:
            case EnemyState.Dead:
            case EnemyState.DeadBody:
                moveSpeed = 0f;
                break;
        }
        animator.SetFloat("Speed", moveSpeed);
    }

    // 상태 변경
    private void ChangeState(EnemyState state)
    {
        if (currentState == state) return;
        currentState = state;
        enemyCombat.StopAttack();

        switch (state)
        {
            // 순찰
            case EnemyState.Patrol:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = patrolSpeed;
                break;

            // 대기
            case EnemyState.Idle:
                idleTimer = idleTime;
                navMeshAgent.isStopped = true;
                break;

            // 경계
            case EnemyState.Alert:
                alertTimer = alertTime;
                alertDecided = false;
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                break;

            // 수색
            case EnemyState.Search:
                searchTimer = searchTime;
                navMeshAgent.isStopped = false;
                SetRandomSearchPosition();
                break;

            // 추적
            case EnemyState.Chase:
                navMeshAgent.speed = chaseSpeed;
                navMeshAgent.isStopped = false;
                break;

            // 공격
            case EnemyState.Attack:
                navMeshAgent.isStopped = true;
                enemyCombat.KeepAttackAnimation(true);
                break;

            // 장전
            case EnemyState.Reload:
                navMeshAgent.isStopped = true;
                enemyCombat.Reload();
                break;

            // 복귀
            case EnemyState.Return:
                navMeshAgent.isStopped = false;
                break;
        }
    }

    // 상태 업데이트
    private void UpdateState()
    {
        if (currentState == EnemyState.Dead)
        {
            UpdateDead();
            return;
        }

        if (currentState == EnemyState.DeadBody)
        { 
            UpdateDeadBody();
            return;
        }

        switch (currentState)
        {
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Alert: UpdateAlert(); break;
            case EnemyState.Search: UpdateSearch(); break;
            case EnemyState.Chase: UpdateChase(); break;
            case EnemyState.Attack: UpdateAttack(); break;
            case EnemyState.Reload: UpdateReload(); break;
            case EnemyState.Return: UpdateReturn(); break;
        }
    }
    #endregion

    #region 적 AI 상태별 로직 (순찰 - 대기 - 경계 - 수색 - 추적 - 공격 - 복귀 - 사망 - 시체)
    // 순찰 상태
    private void UpdatePatrol()
    {
        if (patrolPoint == null || patrolPoint.Length == 0) return;
        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh) return;

        // 순찰 경로 이동 유지
        navMeshAgent.SetDestination(patrolPoint[patrolIndex].position);

        if (navMeshAgent.pathPending) return;
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) return;

        // 인덱스 순 이동
        if (patrolForward)
        {
            patrolIndex++;
            // 마지막 순찰 포인트 도착
            if (patrolIndex >= patrolPoint.Length - 1)
            {
                patrolIndex = patrolPoint.Length - 1;
                patrolForward = false;
            }
        }
        // 인덱스 역순 이동
        else
        {
            patrolIndex--;
            // 시작 순찰 포인트 도착
            if (patrolIndex <= 0)
            {
                patrolIndex = 0;
                patrolForward = true;
            }
        }
        ChangeState(EnemyState.Idle);
    }

    // 대기 상태
    private void UpdateIdle()
    {
        navMeshAgent.isStopped = true;
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            navMeshAgent.isStopped = false;
            ChangeState(EnemyState.Patrol);
        }
    }

    // 경계 상태
    private void UpdateAlert()
    {
        alertTimer -= Time.deltaTime;

        if (alertTimer > 0f) return;
        if (alertDecided) return;
        alertDecided = true;

        // 경계 종료
    
        if (enemyVision.CanSeePlayer())
        {
            ChangeState(EnemyState.Attack);
        }
        else
        {
            ChangeState(EnemyState.Chase);
        }
    
    }

    // 수색 상태
    private void UpdateSearch()
    {
        searchTimer -= Time.deltaTime;

        // 플레이어 발견
        if (enemyVision.CanSeePlayer())
        {
            ChangeState(EnemyState.Chase);
        }

        // 수색 종료
        if (searchTimer <= 0f)
        {
            ChangeState(EnemyState.Return);
        }
    }

    // 추적 상태
    private void UpdateChase()
    {
        if (player == null) return;

        Vector3 targetPos = player.position;
        targetPos.y = transform.position.y;

        // ★ 핵심: 보정된 위치 사용
        navMeshAgent.SetDestination(targetPos);

        if (enemyVision.CanSeePlayer())
        {
            float distance = Vector3.Distance(transform.position, player.position);
            Debug.Log("거리 : " + distance);
            Debug.Log("공격거리 : " + enemyCombat.GetAttackDistance());
            if (distance <= enemyCombat.GetAttackDistance())
            {
                Debug.Log("공격 진입");
                ChangeState(EnemyState.Attack);
                return;
            }
        }
    }

    // 공격
    private void UpdateAttack()
    {
        if (player == null) return;

        // 플레이어 보기
        Vector3 lookpos = player.position - transform.position;
        lookpos.y = 0f;

        if (lookpos.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        // 플레이어 놓침
        if (!enemyVision.CanSeePlayer())
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // 공격 거리 벗어남
        if (distance > enemyCombat.GetAttackDistance())
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        // 총알 부족
        if (weaponController.GetCurrentAmmo() <= 0)
        {
            enemyCombat.StopAttack();
            if (weaponController.GetReserveAmmo() > 0)
            {
                ChangeState(EnemyState.Reload);
            }
            return;
        }
        // 공격 실행
        enemyCombat.Attack();
    }

    // 장전 상태
    private void UpdateReload()
    {
        // 최초 진입 시 한번만 실행
        //Debug.Log("Reload State");

        if (enemyCombat.IsReload())
        {
            //Debug.Log("Still Reloading");
            return;
        }

        Debug.Log("Reload Finished");

        // 장전 끝
        if (enemyVision.CanSeePlayer())
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= enemyCombat.GetAttackDistance())
            {
                ChangeState(EnemyState.Attack);
            }
            else
            {
                ChangeState(EnemyState.Chase);
            }
        }
        else
        {
            ChangeState(EnemyState.Search);
        }
    }

    // 복귀 상태
    private void UpdateReturn()
    {
        navMeshAgent.SetDestination(startPosition);

        if (Vector3.Distance(transform.position, startPosition) < 1f)
        {
            ChangeState(EnemyState.Patrol);
        }
    }

    // 사망 상태
    private void UpdateDead()
    { 
        deadTimer -= Time.deltaTime;

        if (deadTimer <= 0f)
        {
            navMeshAgent.enabled = false;
            ChangeState(EnemyState.DeadBody);
        }
    }

    // 시체 상태
    private void UpdateDeadBody()
    {
        navMeshAgent.isStopped = true;
    }
    #endregion

    // 의심 / 경계 UI
    private void UpdateStateUI()
    {
        bool isAlert =
            currentState == EnemyState.Alert ||
            currentState == EnemyState.Chase ||
            currentState == EnemyState.Attack ||
            currentState == EnemyState.Search;

        bool isSuspect =
            !isAlert && detectGauge > 0f && detectGauge < detectMax;

        if (headSuspectIcon != null)
        {
            headSuspectIcon.SetActive(isSuspect);
        }

        if (headAlertIcon != null)
        {
            headAlertIcon.SetActive(isAlert);
        }
    }

    // 순찰 경로
    public void SetPatrolRoute(Transform[] route)
    {
        patrolPoint = route;

        patrolIndex = 0;
        patrolForward = true;

        if (navMeshAgent != null &&
            navMeshAgent.enabled &&
            navMeshAgent.isOnNavMesh &&
            patrolPoint != null &&
            patrolPoint.Length > 0)
        {
            navMeshAgent.SetDestination(patrolPoint[0].position);
        }
    }

    // 랜덤 수색 위치 설정
    private void SetRandomSearchPosition()
    {
        float randomX = Random.Range(-searchRange, searchRange);
        float randomZ = Random.Range(-searchRange, searchRange);

        Vector3 randomPosition = lastSeenPosition + new Vector3(randomX, 0f, randomZ);
        NavMeshHit navMeshHit;

        if (NavMesh.SamplePosition(randomPosition, out navMeshHit, searchRange, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(navMeshHit.position);
        }
    }

    public void OnReload()
    {
        if (enemyVision.CanSeePlayer())
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= enemyCombat.GetAttackDistance())
            {
                ChangeState(EnemyState.Attack);
            }
            else
            {
                ChangeState(EnemyState.Chase);
            }
        }
        else
        {
            ChangeState(EnemyState.Search);
        }
    }

    // 사망 처리
    public void OnDead()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.DeadBody) return;

        currentState = EnemyState.Dead;
        MissionManager.Instance?.AddKill();

        detectGauge = 0f; // 게이지 0처리로 죽을때 머리위의 물음표 느낌표 제거

        headSuspectIcon.SetActive(false);
        headAlertIcon.SetActive(false);

        // NavMesh 존재할 때만
        if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
        }

        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }
        deadTimer = 2f;
    }
}