using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    public static PlayerLoadout Instance;

    [Header("¹«±â ½½·Ô")]
    public WeaponData weaponSlot1;
    public WeaponData weaponSlot2;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        if (weaponSlot1 == null && weaponSlot2 == null)
        {
            weaponSlot1 = null;
            weaponSlot2 = null;
        }
    }
}
