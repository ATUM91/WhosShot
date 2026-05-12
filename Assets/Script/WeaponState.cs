using UnityEngine;

public enum WeaponType
{
    Pistol, Rifle, Shotgun
}

public class WeaponState : MonoBehaviour
{
    // 현재 들고 있는 무기 상태
    public WeaponType currentWeapon;
}
