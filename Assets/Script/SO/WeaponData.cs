using UnityEngine;

// 무기 종류 구분용
// 무기 데이터 저장

public enum WeaponType
{ 
    Pistol, Rifle
}

[System.Serializable]
[CreateAssetMenu(fileName = "WeaponData", menuName = "FPS/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName;       // 무기 이름
    public WeaponType weaponType;   // 무기 종류

    [Header("프리팹")]
    public GameObject weaponPrefab;

    [Header("UI")]
    public Sprite weaponIcon;

    [Header("전투")]
    public float damage = 10f;      // 데미지
    public float fireRate = 0.2f;   // 초당 발사 수
    public float range = 100f;      // 사거리

    [Header("탄약")]
    public int magazineAmmo = 30;   // 탄창 크기
    public int maxReserveAmmo = 90; // 최대 예비탄
    public float reloadTime = 2f;   // 장전 시간

    [Header("탄퍼짐")]
    public float spreaBurst = 0.005f;       // 연사 시 증가량
    public float spreadRecoverSpeed = 4f;   // 탄퍼짐 복구 속도
    public float maxSpread = 0.05f;         // 최대 탄퍼짐

    [Header("소음")]
    public float noiseRadius = 10f;     // AI 감지 범위
    public bool canUseSilencer = false; // 소음기 가능 여부

    [Header("반동")]
    public float recoilX;
    public float recoilY;

    [Header("애니메이터")]   // 총기 종류별로 알맞는 컨트롤러 넣기
    public RuntimeAnimatorController baseController;        // 라이플
    public AnimatorOverrideController overrideController;   // 피스톨

    [Header("사운드")]
    public AudioClip fireSound;     // 발사 사운드
    public AudioClip reloadSound;   // 장전 사운드
    public AudioClip equipSound;    // 무기 교체 사운드
    public float fireVolume = 1f;   // 발사 볼륨 (SO에서 개별 조절)
    public float reloadVolume = 1f; // 장전 볼륨 (SO에서 개별 조절)
    public float equipVolume = 1f;  // 무기 교체 볼륨 (SO에서 개별 조절)
}
