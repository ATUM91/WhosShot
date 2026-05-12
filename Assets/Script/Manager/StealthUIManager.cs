using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 스텔스모드 HUD 관리
// 크로스헤어 / 상호작용 UI 관리

public class StealthUIManager : MonoBehaviour
{
    public static StealthUIManager Instance;
    [Header("크로스헤어")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite[] crosshairSprite;

    [Header("상호작용 UI")]
    [SerializeField] private TMP_Text interactionText;

    void Start()
    {
        ApplyCrosshair();
        // 시작 시 숨김 처리
        interactionText.gameObject.SetActive(false);
    }

    // 저장된 크로스헤어 적용
    public void ApplyCrosshair()
    {
        // 저장된 번호 가져오기
        int index = SettingManager.Instance.crosshairIndex;
        // 범위 체크
        if (index < 0 || index >= crosshairSprite.Length)
        {
            index = 0;
        }
        // UI 반영
        crosshairImage.sprite = crosshairSprite[index];
    }

    // 상호작용 UI 표시
    public void ShowInteraction(string text)
    {
        interactionText.gameObject.SetActive(true);
        interactionText.text = text;
    }

    // 상호작용 UI 숨김
    public void BlindInteraction()
    {
        interactionText.gameObject.SetActive(false);
    }
}
