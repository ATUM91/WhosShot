using UnityEngine;

// ÃÑ¾ËÀÚ±¹, ÇÇ Æ¦ ¿ÀºêÁ§Æ® Ç®¸µ
// BulletHole, Blood ÇÁ¸®ÆÕ Àç»ç¿ë
// Instantiate ³²¹ß ¹æÁö

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance;

    [Header("ÀÌÆåÆ® ÇÁ¸®ÆÕ / Ç® °³¼ö")]
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject muzzleFirePrefab;
    [SerializeField] private int effectPoolSize = 50;

    private GameObject[] bulletHolePool;    // ÃÑ¾ËÀÚ±¹ Ç®
    private GameObject[] bloodPool;         // ÇÇ Æ¦ Ç®
    private GameObject[] muzzleFirePool;    // ÃÑ±¸ ºÒ²É Ç®
    
    private int bulletHoleIndex;
    private int bloodIndex;
    private int muzzleFireIndex;

    void Awake()
    {
        Instance = this;
        CreateEffectPool();
    }

    // ÀÌÆåÆ® Ç® »ý¼º
    private void CreateEffectPool()
    {
        bulletHolePool = new GameObject[effectPoolSize];
        bloodPool = new GameObject[effectPoolSize];
        muzzleFirePool = new GameObject[effectPoolSize];

        for (int i = 0; i < effectPoolSize; i++)
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, transform);
            bulletHole.SetActive(false);
            bulletHolePool[i] = bulletHole; // ÃÑ¾Ë ÀÚ±¹

            GameObject blood = Instantiate(bloodPrefab, transform);
            blood.SetActive(false);
            bloodPool[i] = blood; // ÇÇ Æ¦

            GameObject muzzleFire = Instantiate(muzzleFirePrefab, transform);
            muzzleFire.SetActive(false);
            muzzleFirePool[i] = muzzleFire; // ÃÑ±¸ ºÒ²É 
        }
    }

    // ÃÑ¾Ë ÀÚ±¹ °¡Á®¿À±â
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

    // ÇÇ Æ¦ °¡Á®¿À±â
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

    // ÃÑ±¸ ºÒ²É °¡Á®¿À±â
    public GameObject GetMuzzleFire()
    { 
        GameObject muzzleFire = muzzleFirePool[muzzleFireIndex];
        muzzleFireIndex++;

        if (muzzleFireIndex >= muzzleFirePool.Length)
        { 
            muzzleFireIndex = 0;
        }
        muzzleFire.SetActive(false);
        muzzleFire.SetActive(true);
        return muzzleFire;
    }
}
