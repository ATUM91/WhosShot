using UnityEngine;

// 무기 관리
// 발사 처리
// 연사 / 샷건 / 소음 처리

public class WeaponController : MonoBehaviour
{
    public enum WeaponType
    {
        Pistol,
        Rifle,
        Shotgun
    }

    [Header("카메라")]
    [SerializeField] private Camera playerCamera;   // 발사 기준 -> 카메라

    [Header("현재 무기")]
    [SerializeField] private WeaponType weaponType; // 현재 무기 타입

    [Header("무기 스펙")]
    [SerializeField] private float damage = 20f;    // 기본 데미지
    [SerializeField] private float range = 100f;    // 사거리
    [SerializeField] private float fireRate = 10f;  // 초당 발사 수

    [Header("샷건")]
    [SerializeField] private int pelletCount = 8; // 샷건 탄 수
    [SerializeField] private float spread = 0.2f;   // 탄퍼짐 정도

    [Header("소음기")]
    [SerializeField] private float noiseRadius = 10f;     // 기본 소음 범위
    [SerializeField] private bool canUseSilencer = false; // 소음기 장착 가능 여부

    [Header("전투 시스템")]
    [SerializeField] private Battle battle; // 데미지 계산 전달

    private float nextFireTime;         // 다음 발사 가능 시간
    private bool isSilencer = false;    // 소음기 장착 여부

    void Update()
    {
        // 좌클릭 유지 시 연사
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            Fire();
        }
    }

    // 발사 실행 함수
    private void Fire()
    {
        // 무기 타입에 따른 발사 방식
        switch (weaponType)
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
        // 발사 후 소음 발생
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
        for (int i = 0; i < pelletCount; i++)
        {
            // 기본 방향
            Vector3 direction = playerCamera.transform.forward;
            // 랜덤 탄 퍼짐 적용
            direction += Random.insideUnitSphere * spread;

            Shoot(direction);
        }
    }

    // Raycast + Battle로 전달
    private void Shoot(Vector3 direction)
    {
        Ray ray = new Ray(playerCamera.transform.position, direction);
        RaycastHit hit;

        // Raycast 충돌 검사
        if (Physics.Raycast(ray, out hit, range))
        {
            // Battle로 충돌 정보 + 데미지 전달
            battle.Hit(hit, damage);
        }
    }

    // 소음 처리
    private void MakeNoise()
    {
        // 소음기 장착 + 사용 가능 무기라면 AI 감지 차단
        if (isSilencer && canUseSilencer) return;

        // AIManager에 소리 전달 로직 필요
    }

    // 소음기 장착 / 해제
    private void SetSilencer(bool value)
    {
        // 해당 무기가 소음기 사용 가능할 때만 적용
        if (canUseSilencer)
        {
            isSilencer = value;
        }
    }



}
