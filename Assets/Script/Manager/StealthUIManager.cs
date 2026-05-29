using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("HP UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

    [Header("탄약 UI")]
    [SerializeField] private TMP_Text ammoText;

    [Header("발각 게이지")]
    [SerializeField] private GameObject detectGauge;
    [SerializeField] private Image detectFill;
    [SerializeField] private Image screenSuspectIcon;
    [SerializeField] private Image screenAlertIcon;

    [Header("시체 UI")]
    [SerializeField] private GameObject deadBodyHoldUI;
    [SerializeField] private Image deadBodyHoldFill;

    [Header("미션 UI")]
    [SerializeField] private GameObject missionHoldUI;
    [SerializeField] private Image missionHoldFill;

    [Header("결과 UI")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        ApplyCrosshair();
        // 시작 시 숨김 처리
        interactionText.gameObject.SetActive(false);

        detectGauge.SetActive(false);

        screenSuspectIcon.gameObject.SetActive(false);
        screenAlertIcon.gameObject.SetActive(false);

        successPanel.SetActive(false);
        failPanel.SetActive(false);
    }

    void Update()
    {
        UpdateDetectGauge();
    }

    #region 고정 HUD
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

    // HP UI 표시
    public void UpdateHP(float currentHP, float maxHP)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHP:0}/{maxHP:0}";
        }
    }

    // 탄약 UI 표시
    public void UpdateAmmo(int currentAmmo, float reserveAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}/{reserveAmmo}";
        }
    }
    #endregion

    #region 상호작용 UI
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

    // 시체 UI ON
    public void ShowDeadBodyHold()
    {
        if (deadBodyHoldUI == null) return;
        deadBodyHoldUI.SetActive(true);
    }

    // 시체 UI OFF
    public void HideDeadBodyHold()
    {
        if (deadBodyHoldUI == null) return;
        deadBodyHoldUI.SetActive(false);
    }

    // 미션 UI ON
    public void ShowMissionHold()
    {
        if (missionHoldUI == null) return;
        missionHoldUI.SetActive(true);
    }

    // 미션 UI OFF
    public void HideMissionHold()
    {
        if (missionHoldUI == null) return;
        missionHoldUI.SetActive(false);
    }
    #endregion

    // 시체 진행률 업데이트 (0~1)
    public void UpdateDeadBodyHold(float ratio)
    {
        if (deadBodyHoldFill == null) return;
        deadBodyHoldFill.fillAmount = Mathf.Clamp01(ratio);
    }

    // C4 진행률 업데이트 (0~1)
    public void UpdateMissionHold(float ratio)
    {
        if (missionHoldFill == null) return;

        missionHoldFill.fillAmount = ratio;
    }

    // 발각 게이지 표시
    public void UpdateDetectGauge()
    {
        if (DetectManager.Instance == null) return;

        float currentGauge = DetectManager.Instance.GetDetect();
        float maxGauge = DetectManager.Instance.GetMaxDetect();

        float ratio = currentGauge / maxGauge;
        bool isDetecting = currentGauge > 0f;

        detectGauge.SetActive(isDetecting);
        if (!isDetecting) return;

        detectFill.fillAmount = ratio;

        // 게이지가 꽉 차면 느낌표로 전환
        if (ratio >= 1f)
        {
            screenSuspectIcon.gameObject.SetActive(false);
            screenAlertIcon.gameObject.SetActive(true);
        }
        else
        {
            screenSuspectIcon.gameObject.SetActive(true);
            screenAlertIcon.gameObject.SetActive(false);
        }
    }

    #region 성공 실패 패널 UI
    // 결과 UI
    public void ShowSuccess()
    { 
        successPanel.SetActive(true);
        failPanel.SetActive(false);
        Time.timeScale = 0f;
        SetUIPanel(true);
    }

    public void ShowFail()
    { 
        failPanel.SetActive(true);
        successPanel.SetActive(false);
        Time.timeScale = 0f;
        SetUIPanel(true);
    }

    // 로비 이동
    public void GoLobby()
    {
        Time.timeScale = 1f;
        SetUIPanel(false);
        SceneLoading.LoadTo("Scene Lobby");
    }
    #endregion

    public void SetUIPanel(bool isPanel)
    {
        if (isPanel)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        { 
            Cursor.lockState= CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
