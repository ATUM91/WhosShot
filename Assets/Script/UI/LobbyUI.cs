using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 로비 UI 관리
// 각 버튼 클릭에 따라 패널 전환 처리

public class LobbyUI : MonoBehaviour
{
    [Header("맵 선택")]
    [SerializeField] private Toggle oilStorageToggle;
    [SerializeField] private Toggle supplyCenterToggle;

    [SerializeField] private string oilStorageSceneName;
    [SerializeField] private string supplyCenterSceneName;

    [Header("메인 패널")]
    public GameObject mainPanel;                // 로비(메인)
    public GameObject stealthMapSelectPanel;    // 스텔스 맵 선택 패널
    public GameObject tdmMapSelectPanel;        // 팀데스매치 맵 선택 패널
    public GameObject weaponSelectPanel;        // 무기고 선택 패널

    [Header("무기 카테고리")]
    public GameObject pistolPanel;  // 권총 패널
    public GameObject riflePanel;   // 소총 패널
    public GameObject shotgunPanel; // 샷건 패널

    [Header("팝업")]
    public GameObject warningPanel; // 경고 패널
    public GameObject helpPanel;    // 도움말 패널
    public GameObject settingPanel; // 설정 패널
    public GameObject quitPanel;    // 종료 확인 패널

    [Header("무기 장착 패널")]
    [SerializeField] private GameObject equipPanel;

    [Header("무기 슬롯1 UI")]
    [SerializeField] private Image slot1Image;
    [SerializeField] private TMP_Text slot1Text;

    [Header("무기 슬롯2 UI")]
    [SerializeField] private Image slot2Image;
    [SerializeField] private TMP_Text slot2Text;

    // 현재 선택한 무기
    private WeaponData currentWeaponData;

    void Start()
    {
        // 로비씬 진입시 마우스 포인트 활성화
        CursorManager.CursorUnlock();

        equipPanel.SetActive(false);
        
        if (PlayerLoadout.Instance != null)
        {
            PlayerLoadout.Instance.Init();
        }
        RefreshSlotUI();
    }

    // 모든 패널 비활성화
    private void CloseAllPanel()
    { 
        stealthMapSelectPanel.SetActive(false);
        tdmMapSelectPanel.SetActive(false);
        weaponSelectPanel.SetActive(false);
        settingPanel.SetActive(false);
        helpPanel.SetActive(false);
        quitPanel.SetActive(false);

    }

    #region 메인 버튼
    // 스텔스 모드 선택
    public void OnClickStealthSelect()
    {
        CloseAllPanel();
        stealthMapSelectPanel.SetActive(true);

        // 이전 선택 값 초기화 / 토글 꼬임 방지
        oilStorageToggle.isOn = false;
    }
    
    // 팀데스매치 모드 선택
    public void OnClickTDMSelect()
    {
        CloseAllPanel();
        tdmMapSelectPanel.SetActive(true);

        // 이전 선택 값 초기화 / 토글 꼬임 방지
        supplyCenterToggle.isOn = false;
    }
    
    // 무기고 열기
    public void OnClickWeaponSelect()
    {
        CloseAllPanel();
        weaponSelectPanel.SetActive(true);
        // 무기고 열 때 기본은 권총 탭
        OnClickPistolTap();
    }
    #endregion

    #region 무기 카테고리 창
    // 권총 탭
    public void OnClickPistolTap()
    { 
        pistolPanel.SetActive(true);
        riflePanel.SetActive(false);
        shotgunPanel.SetActive(false);
    }

    // 소총 탭
    public void OnClickRifleTap()
    {
        pistolPanel.SetActive(false);
        riflePanel.SetActive(true);
        shotgunPanel.SetActive(false);
    }

    // 샷건 탭
    public void OnClickShotgunTap()
    {
        pistolPanel.SetActive(false);
        riflePanel.SetActive(false);
        shotgunPanel.SetActive(true);
    }
    #endregion

    #region 무기 장착
    // 무기 버튼 클릭
    public void SelectWeapon(WeaponData weaponData)
    {
        currentWeaponData = weaponData;
        equipPanel.SetActive(true);
    }

    // 슬롯1 장착
    public void EquipFirstSlot()
    {
        if (currentWeaponData == null)
        {
            Debug.Log("currentWeaponData NULL");
            return;
        }
        PlayerLoadout.Instance.weaponSlot1 = currentWeaponData;

        RefreshSlotUI();
        equipPanel.SetActive(false);
    }

    // 슬롯2 장착
    public void EquipSecondSlot()
    {
        if (PlayerLoadout.Instance == null) return;
        PlayerLoadout.Instance.weaponSlot2 = currentWeaponData;

        RefreshSlotUI();
        equipPanel.SetActive(false);
    }

    // 장착 패널 취소 버튼
    public void CancleSlotPanel()
    {
        equipPanel.SetActive(false);
    }

    // 슬롯 UI 갱신
    private void RefreshSlotUI()
    {
        if (PlayerLoadout.Instance == null) return;

        // 슬롯1
        if (PlayerLoadout.Instance.weaponSlot1 != null)
        {
            slot1Text.text = PlayerLoadout.Instance.weaponSlot1.weaponName;
            slot1Image.sprite = PlayerLoadout.Instance.weaponSlot1.weaponIcon;
        }
        else
        {
            slot1Text.text = "EMPTY";
            slot1Image.sprite = null;
        }


        // 슬롯2
        if (PlayerLoadout.Instance.weaponSlot2 != null)
        {
            slot2Text.text = PlayerLoadout.Instance.weaponSlot2.weaponName;
            slot2Image.sprite = PlayerLoadout.Instance.weaponSlot2.weaponIcon;
        }
        else
        {
            slot2Text.text = "EMPTY";
            slot2Image.sprite = null;
        }
    }
    #endregion

    #region 팝업 창
    // 경고창 열기
    public void OpenWarning()
    {
        if (warningPanel == null) return;
        warningPanel.SetActive(true);
    }

    // 경고창 닫기
    public void CloseWarning()
    {
        if (warningPanel == null) return;
        warningPanel.SetActive(false);
    }

    // 설정 창 열기
    public void OnClickSetting()
    {
        CloseAllPanel();
        settingPanel.SetActive(true);
    }
    
    // 도움말 창 열기
    public void OnClickHelp()
    {
        CloseAllPanel();
        helpPanel.SetActive(true);
    }
    
    // 종료 확인 창 열기
    public void OnClickQuitInfo()
    {
        CloseAllPanel();
        quitPanel.SetActive(true);
    }
    #endregion

    #region 공통 버튼
    // 뒤로가기(메인으로)
    public void OnClickBackToMain()
    { 
        CloseAllPanel();
        mainPanel.SetActive(true);
    }

    // 게임 시작 / 맵이 추가 될 때 코드 추가
    public void OnClickStartGame()
    {
        // 무기 장착 여부 체크
        if (PlayerLoadout.Instance == null) return;

        bool hasWeapon = PlayerLoadout.Instance.weaponSlot1 != null && PlayerLoadout.Instance.weaponSlot2 != null;
        if (!hasWeapon)
        {
            OpenWarning();
            return;
        }

        // 열려있는 패널 기준으로만 검사.
        if (stealthMapSelectPanel.activeSelf)
        {
            if (oilStorageToggle.isOn)
            {
                SceneLoading.LoadTo(oilStorageSceneName);
                return;
            }
        }
        else if (tdmMapSelectPanel.activeSelf)
        {
            if (supplyCenterToggle.isOn)
            {
                SceneLoading.LoadTo(supplyCenterSceneName);
                return;
            }
        }
    }

    // 게임 종료
    public void OnClickQuitGame()
    {
        Application.Quit();
    }
    #endregion

    #region SoundManager 버튼 SFX 호출용
    public void PlaySFX(AudioClip audioClip)
    { 
        SoundManager.Instance.PlaySFX(audioClip);
    }
    #endregion
}
