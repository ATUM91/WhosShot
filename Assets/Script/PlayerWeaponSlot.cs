using Unity.VisualScripting;
using UnityEngine;

// 플레이어 무기 슬롯 관리
// 1번 2번 무기 장착
// 현재 무기 활성화

public class PlayerWeaponSlot : MonoBehaviour
{
    [Header("무기 위치")]
    [SerializeField] private Transform slot1;
    [SerializeField] private Transform slot2;

    [Header("현재 무기 오브젝트")]
    private GameObject object1;
    private GameObject object2;

    // 캐싱 WeaponController
    private WeaponController wc1;
    private WeaponController wc2;

    // 현재 들고 있는 무기
    private WeaponController currentWeapon;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<PlayerController>().animator;
        // 런타임 데이터 초기화
        WeaponLoadout();
    }

    private void WeaponLoadout()
    {
        SpawnWeapon(PlayerLoadout.Instance.weaponSlot1, slot1, true);
        SpawnWeapon(PlayerLoadout.Instance.weaponSlot2, slot2, false);

        SetWeapon(1);
    }

    // 무기 생성
    private void SpawnWeapon(WeaponData data, Transform parent, bool isSlot1)
    {
        if (data == null) return;

        GameObject obj = Instantiate(data.weaponPrefab);
        obj.transform.SetParent(parent, false);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        WeaponController wc = obj.GetComponent<WeaponController>();
        wc.SetWeaponData(data);
        wc.SetCamera(Camera.main);

        if (isSlot1)
        {
            object1 = obj;
            wc1 = wc;
        }
        else
        {
            object2 = obj;
            wc2 = wc;
        }
    }

    // 무기 스왑 함수
    public void SetWeapon(int index)
    {
        // 전체 OFF
        if (object1 != null) object1.SetActive(false);
        if (object2 != null) object2.SetActive(false);

        currentWeapon = null;

        // 선택 ON
        if (index == 1 && wc1 != null)
        {
            object1.SetActive(true);
            currentWeapon = wc1;
            wc1.PlayEquipSound();
            wc1.ApplyAnimator(animator);
        }

        else if (index == 2 && wc2 != null)
        {
            object2.SetActive(true);
            currentWeapon = wc2;
            wc2.PlayEquipSound();
            wc2.ApplyAnimator(animator);
        }
    }

    // 슬롯1 장착
    public void EquipSlot1() { SetWeapon(1); }
    // 슬롯2 장착
    public void EquipSlot2() { SetWeapon(2); }

    // 현재 무기 반환
    public WeaponController GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
