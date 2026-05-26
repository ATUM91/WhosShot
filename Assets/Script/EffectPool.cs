using UnityEngine;

// 총알자국, 피 튐 오브젝트 풀링
// BulletHole, Blood 프리팹 재사용
// Instantiate 남발 방지

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance;

    [Header("이펙트 프리팹 / 풀 개수")]
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private int effectPoolSize = 30;

    private GameObject[] bulletHolePool;    // 총알자국 풀
    private GameObject[] bloodPool;         // 피 튐 풀
    
    private int bulletHoleIndex;
    private int bloodIndex;

    void Awake()
    {
        Instance = this;
        CreateEffectPool();
    }

    // 이펙트 풀 생성
    private void CreateEffectPool()
    {
        bulletHolePool = new GameObject[effectPoolSize];
        bloodPool = new GameObject[effectPoolSize];

        for (int i = 0; i < effectPoolSize; i++)
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, transform);
            bulletHole.SetActive(false);
            bulletHolePool[i] = bulletHole; // 총알 자국

            GameObject blood = Instantiate(bloodPrefab, transform);
            blood.SetActive(false);
            bloodPool[i] = blood; // 피 튐
        }
    }

    // 총알 자국 가져오기
    public GameObject GetBulletHole()
    {
        GameObject bulletHole = bulletHolePool[bulletHoleIndex];
        bulletHoleIndex++;

        if (bulletHoleIndex >= bulletHolePool.Length)
        {
            bulletHoleIndex = 0;
        }
        bulletHole.SetActive(false);
        bulletHole.SetActive(true);
        return bulletHole;
    }

    // 피 튐 가져오기
    public GameObject GetBlood()
    {
        GameObject blood = bloodPool[bloodIndex];
        bloodIndex++;

        if (bloodIndex >= bloodPool.Length)
        {
            bloodIndex = 0;
        }
        blood.SetActive(false);
        blood.SetActive(true);
        return blood;
    }
}
