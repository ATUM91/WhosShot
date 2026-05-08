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

    [Header("참조")]
    [SerializeField] private Camera playerCamera;   // 발사 기준 카메라
    [SerializeField] private Animator animator;        // 플레이어 애니메이션
    [SerializeField] private Battle battle;         // 데미지 계산 전달용

    private int currentAmmo;
    private float nextFireTime;         // 다음 발사 가능 시간 / 연사 제한
    private bool isSilencer;

    void Start()
    {
        currentAmmo = weaponData.maxAmmo;
    }

    // 발사 실행 함수
    public void Fire()
    {
        if (currentAmmo <= 0) return;
        if (Time.time < nextFireTime) return; // 연사 속도 제한

        nextFireTime = Time.time + (1f / weaponData.fireRate); // 다음 발사 시간 갱신
        animator.SetTrigger("Shot"); // 애니메이션 실행
        animator.SetInteger("WeaponType", (int)weaponData.weaponType); // 무기타입 애니메이터 전달

        currentAmmo--; // 탄약 감소

        // 무기 타입에 따른 발사 방식
        switch (weaponData.weaponType)
        {
            case WeaponType.Pistol:
                FireSingle();
                break;

            case WeaponType.Rifle:
                FireSingle();
                break;

            case WeaponType.Shotgun:
                FireShotgun();
                break;
        }
        // 소음 처리
        MakeNoise();
    }

    // 단발
    private void FireSingle()
    {
        // 카메라 정면 방향
        Shoot(playerCamera.transform.forward);
    }

    // 샷건
    private void FireShotgun()
    {
        for (int i = 0; i < weaponData.pelletCount; i++)
        {
            // 기본 방향
            Vector3 direction = playerCamera.transform.forward;
            // 랜덤 탄 퍼짐 적용
            direction += Random.insideUnitSphere * weaponData.spread;

            Shoot(direction);
        }
    }

    // Raycast + Battle로 전달
    private void Shoot(Vector3 direction)
    {
        Ray ray = new Ray(playerCamera.transform.position, direction);
        RaycastHit hit;

        // Raycast 충돌 검사
        if (Physics.Raycast(ray, out hit, weaponData.range))
        {
            // Battle로 충돌 정보 + 데미지 전달
            battle.Hit(hit, weaponData.damage);
            // 디버그 확인용
            Debug.LogError(hit.collider.name);
        }
    }

    // 소음 처리
    private void MakeNoise()
    {
        // 소음기 장착 + 사용 가능 무기라면 AI 감지 차단
        if (isSilencer && weaponData.canUseSilencer) return;

        // AIManager에 소리 전달 로직 필요
    }

    // 소음기 장착 / 해제
    private void SetSilencer(bool value)
    {
        // 해당 무기가 소음기 사용 가능할 때만 적용
        if (weaponData.canUseSilencer)
        {
            isSilencer = value;
        }
    }



}
