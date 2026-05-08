using UnityEngine;
using UnityEngine.UI;

// UI 시각적 요소만 처리 (크로스헤어 포함)
// SettingManager 값 반영

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("크로스헤어")]
    [SerializeField] private Image crosshairImage;      // 현재 크로스헤어 UI
    [SerializeField] private Sprite[] crosshairSprites; // 크로스헤어 목록

    private int currentIndex = -1; // 현재 적용된 크로스헤어 인덱스

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        ApplyCrosshair(); // 시작할 때 1회 적용
    }

    // 크로스헤어 적용
    public void ApplyCrosshair()
    {
        currentIndex = SettingManager.Instance.crosshairIndex;
        // 범위 체크
        if (currentIndex < 0 || currentIndex >= crosshairSprites.Length) return;
        // UI 반영
        crosshairImage.sprite = crosshairSprites[currentIndex];
    }
}
