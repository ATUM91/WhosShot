using UnityEngine;

// 장착 무기 발사 처리
// WeaponData(SO) 기반 무기 데이터 사용
// 단발 / 샷건 분기
// 애니메이션 호출
// Raycast 판정
// 입력처리X / 무기교체X / 재장전X

public class WeaponController : MonoBehaviour
{
    [Header("현재 무기 SO 데이터")]
    [SerializeField] private WeaponData weaponData; // 현재 무기 타입

    [Header("카메라")]
    [SerializeField] private Camera playerCamera;   // 발사 기준 카메라

    private Battle battle;         // 데미지 계산 전달용

    private int currentAmmo;
    private float nextFireTime;         // 다음 발사 가능 시간 / 연사 제한
    private bool isSilencer;

    void Awake()
    {
        // 캐싱
        battle = GetComponent<Battle>();
    }

    // 카메라 설정 (발사 기준)
    public void SetCamera(Camera cam)
    {
        playerCamera = cam;
    }

    // 현재 무기 데이터 반환
    public WeaponData GetWeaponData()
    {
        return weaponData;
    }

    // 무기 데이터 설정
    public void SetWeaponData(WeaponData data)
    {
        weaponData = data;
    }

    // 탄약 설정 (스왑 복구용)
    public void SetAmmo(int ammo)
    {
        currentAmmo = ammo;
    }

    // 탄약 설정 (스왑 저장용)
    public int GetAmmo()
    {
        return currentAmmo;
    }

    // 발사 실행 함수
    public void Fire()
    {
        if (weaponData == null) return;
        if (currentAmmo <= 0) return;
        if (Time.time < nextFireTime) return; // 연사 속도 제한

        nextFireTime = Time.time + (1f / weaponData.fireRate); // 다음 발사 시간 갱신
        currentAmmo--; // 탄약 감소

        Shoot(playerCamera.transform.forward);
        
        // 소음 처리
        MakeNoise();
    }

    private void Shoot(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, dir, out hit, weaponData.range))
        {
            battle.Hit(hit, weaponData.damage);
        }
    }

    // 소음 처리
    private void MakeNoise()
    {
        // 소음기 장착 + 사용 가능 무기라면 AI 감지 차단
        if (isSilencer && weaponData.canUseSilencer) return;

        // AIManager에 소리 전달 로직 필요
    }
}
