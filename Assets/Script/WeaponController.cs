using UnityEngine;

// 장착 무기 발사 처리
// WeaponData(SO) 기반 무기 데이터 사용
// 단발 / 샷건 분기
// 실제 총알 프리팹 발사 -> 히트스캔으로 변경
// 입력처리X / 무기교체X / 재장전X

public class WeaponController : MonoBehaviour
{
    [Header("현재 무기 SO 데이터")]
    [SerializeField] private WeaponData weaponData; // 현재 무기 타입

    [Header("카메라")]
    [SerializeField] private Camera playerCamera;   // 발사 기준 카메라

    [Header("총구 위치")]
    [SerializeField] private Transform muzzlePoint; // 총구 불꽃 이펙트 위치

    private Transform target;
    private Animator animator;
    private AudioSource audioSource;

    private int currentAmmo;    // 현재 탄창
    private int reserveAmmo;    // 예비 탄창

    private float nextFireTime;  // 다음 발사 가능 시간 / 연사 제한
    private float currentSpread;

    private bool isReload;
    public bool _reload_ => isReload;
    private bool isSilencer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        RecoverSpread();
    }

    // 외부 접근용
    public WeaponData GetWeaponData()
    {
        return weaponData;
    }

    // 무기 데이터 설정
    public void SetWeaponData(WeaponData data)
    {
        weaponData = data;
        if (weaponData != null)
        {
            currentAmmo = weaponData.magazineAmmo;
            reserveAmmo = weaponData.maxReserveAmmo;
            currentSpread = 0; // 첫발 정확도
        }
    }

    // 현재 탄창 설정
    public int GetCurrentAmmo() { return currentAmmo; }

    // 예비 탄창 설정
    public int GetReserveAmmo() { return reserveAmmo;}

    // 장전 상태 설정
    public bool IsReload() { return isReload; }

    // 카메라 설정 (발사 기준)
    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }

    public void SetTarget(Transform newTarget)
    {
        Debug.Log("WeaponController SetTarget : " + newTarget);

        target = newTarget;
    }

    // 애니메이터
    public void ApplyAnimator(Animator animator)
    {
        if (weaponData == null) return;
        // 피스톨 적용
        if (weaponData.overrideController != null)
        {
            animator.runtimeAnimatorController = weaponData.overrideController;
        }
        // 라이플 적용
        if (weaponData.baseController != null)
        {
            animator.runtimeAnimatorController = weaponData.baseController;
        }
    }

    public bool CanFire()
    {
        return Time.time >= nextFireTime && currentAmmo > 0;
    }

    public bool CanReload()
    {
        if (weaponData == null) return false;
        if (isReload) return false; // 장전 중 장전 불가
        if (reserveAmmo <= 0) return false; // 예비탄 없으면 장전 불가
        if (currentAmmo >= weaponData.magazineAmmo) return false; // 탄창 가득차면 장전 불가

        return true;
    }

    // 발사 실행 함수
    public void Fire()
    {
        // null 체크
        if (weaponData == null) return;
        if (isReload) return; // 장전중이면 리턴
        if (Time.time < nextFireTime) return; // 연사 속도 제한

        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }
        nextFireTime = Time.time + weaponData.fireRate; // 다음 발사 시간 갱신
        currentAmmo--; // 탄약 감소
        Debug.Log($"1발 발사 / 남은 탄창 : {currentAmmo}발");
        PlayFireSound();
        Shoot();

        currentSpread += weaponData.spreaBurst; // 연속 사격 시 탄퍼짐 증가
        currentSpread = Mathf.Clamp(currentSpread, 0f, weaponData.maxSpread);

        if (GetComponentInParent<PlayerController>() != null)
        {
            StealthUIManager.Instance?.UpdateAmmo(currentAmmo, reserveAmmo);
        }
    }

    // 장전
    public bool Reload()
    {
        if (!CanReload()) return false;

        isReload = true;
        PlayReloadSound();
        return true;
    }

    // 애니메이션 이벤트 전용
    public void FinishReload()
    {
        int needAmmo = weaponData.magazineAmmo - currentAmmo;
        int reloadAmmo = Mathf.Min(needAmmo, reserveAmmo);

        currentAmmo += reloadAmmo;
        reserveAmmo -= reloadAmmo;
        isReload = false;

        if (GetComponentInParent<PlayerController>() != null)
        {
            StealthUIManager.Instance?.UpdateAmmo(currentAmmo, reserveAmmo);
        }
        Debug.Log($"장전 완료 / 탄창 : {currentAmmo}발 / 예비탄 : {reserveAmmo}발");
    }

    // 실제 발사
    private void Shoot()
    {
        // 총구 불꽃 / 발사 이펙트
        GameObject muzzleFire = EffectPool.Instance.GetMuzzleFire();

        muzzleFire.transform.position = muzzlePoint.position;
        muzzleFire.transform.rotation = muzzlePoint.rotation;

        Ray ray;

        // 플레이어
        if (playerCamera != null)
        {
            ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // 화면 중앙 Ray
        }

        // AI
        else
        {
            if (target != null)
            {
                Vector3 targetPos = target.position + Vector3.up * 1.4f;
                Vector3 shootDir = (targetPos - muzzlePoint.position).normalized;
                ray = new Ray(muzzlePoint.position, shootDir);
            }
            else
            {
                ray = new Ray(muzzlePoint.position, muzzlePoint.forward);
            }
        }

        Vector3 targetPoint = ray.GetPoint(100f); // 정확한 기준점을 생성

        Vector3 dir = (targetPoint - ray.origin).normalized; // 총구 -> 목표방향
        dir = ApplySpread(dir); // 탄퍼짐 적용

        int mask = Physics.DefaultRaycastLayers;

        if (playerCamera != null)
        {
            int playerLayer = LayerMask.NameToLayer("Player"); // 플레이어 레이어 제외
            mask = ~(1 << playerLayer);
        }
        RaycastHit hit;
        
        // 레이 시각화
        if (Physics.Raycast(ray.origin, dir, out hit, weaponData.range, mask))
        {
            // 자기 자신이면 무시
            if (hit.collider.transform.root == transform.root) return;
            
            Target target = hit.collider.GetComponentInParent<Target>();

            // Player, Enemy 맞았을 때 / 피 튐 생성
            if (target != null)
            {
                float finalDamage = target.CheckDamage(hit.collider.tag, weaponData.damage);
                target.TakeDamage(finalDamage);

                GameObject blood = EffectPool.Instance.GetBlood();  // 피 튐 이펙트 가져오기
                blood.transform.position = hit.point + hit.normal * 0.02f;   // 위치 적용
                blood.transform.rotation = Quaternion.LookRotation(hit.normal); // 방향 적용
            }
            // 벽 맞았을 때 / 총알 자국 생성
            else
            {
                GameObject bulletHole = EffectPool.Instance.GetBulletHole();
                bulletHole.transform.position = hit.point + hit.normal * 0.01f;
                bulletHole.transform.rotation = Quaternion.LookRotation(-hit.normal);
            }
        }
    }

    // 탄퍼짐 계산
    private Vector3 ApplySpread(Vector3 dir)
    {
        // 탄퍼짐 거의 없으면 정확하게 발사
        if (currentSpread <= 0.0001f) return dir.normalized;

        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        Vector3 up = Vector3.Cross(dir, right).normalized;
        float x = Random.Range(-currentSpread, currentSpread);
        float y = Random.Range(-currentSpread, currentSpread);

        dir += right * x;
        dir += up * y;

        return dir.normalized;
    }

    // 탄퍼짐 복구 / 사격 멈추면 다시 정확히 돌아옴
    private void RecoverSpread()
    {
        if (weaponData == null) return;

        currentSpread = Mathf.Lerp(currentSpread, 0f, weaponData.spreadRecoverSpeed * Time.deltaTime);

        // 거의 0이면 강제로 0처리 / 버그 방지
        if (currentSpread < 0.001f)
        {
            currentSpread = 0f;
        }
    }

    #region 사운드
    // 무기 교체 사운드
    public void PlayEquipSound()
    {
        if (weaponData.equipSound == null) return;
        if (audioSource == null) return;

        audioSource.PlayOneShot(weaponData.equipSound);
    }

    // 발사 사운드
    private void PlayFireSound()
    {
        if (weaponData.fireSound == null) return;
        if (audioSource == null) return;

        audioSource.PlayOneShot(weaponData.fireSound);
    }

    // 장전 사운드
    private void PlayReloadSound()
    {
        if (weaponData.reloadSound == null) return;
        if (audioSource == null) return;

        audioSource.PlayOneShot(weaponData.reloadSound);
    }
    #endregion
}
