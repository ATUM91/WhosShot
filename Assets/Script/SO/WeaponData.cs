using UnityEngine;

// 무기 종류 구분용

public enum WeaponType
{ 
    Pistol, Rifle, Shotgun
}

// 무기 데이터 저장

[CreateAssetMenu(fileName = "WeaponData", menuName = "FPS/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName;       // 무기 이름
    public WeaponType weaponType;   // 무기 종류

    [Header("전투")]
    public float damage = 10f;      // 데미지
    public float fireRate = 0.2f;   // 초당 발사 수
    public float range = 100f;      // 사거리

    [Header("탄약")]
    public int maxAmmo = 30;
    public float reloadTime = 2f;

    [Header("특수 무기")]
    public int pelletCount = 8;     // 샷건 탄 수
    public float spread = 0.2f;     // 탄 퍼짐

    [Header("소음")]
    public float noiseRadius = 10f;     // AI 감지 범위
    public bool canUseSilencer = false; // 소음기 가능 여부

    [Header("반동")]
    public float recoilX;
    public float recoilY;


}
