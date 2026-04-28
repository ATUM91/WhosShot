using UnityEngine;
using UnityEngine.UI;

// UI 처리 (크로스헤어 포함)
// SettingManager 값 반영

public class UIManager : MonoBehaviour
{
    [Header("크로스헤어")]
    [SerializeField] private Image crosshairImage;      // 현재 크로스헤어 UI
    [SerializeField] private Sprite[] crosshairSprites; // 크로스헤어 목록

    private int currentIndex = -1; // 현재 적용된 크로스헤어 인덱스

    void Start()
    {
        ApplyCrosshair(); // 시작할 때 적용
    }

    void Update()
    {
        // 설정 값 변경 감지 -> 크로스헤어 갱신
        if (currentIndex != SettingManager.Instance.crosshairIndex)
        {
            ApplyCrosshair();
        }
    }

    // 크로스헤어 적용
    private void ApplyCrosshair()
    { 
        currentIndex = SettingManager.Instance.crosshairIndex;
        // 범위 체크
        if (currentIndex < 0 || currentIndex >= crosshairSprites.Length) return;

        crosshairImage.sprite = crosshairSprites[currentIndex];
    }
}
