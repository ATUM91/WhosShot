using UnityEngine;
using UnityEngine.UI;

// 로비 UI 관리
// 각 버튼 클릭에 따라 패널 전환 처리

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Toggle oilStorageToggle;
    [SerializeField] private Toggle cityToggle;

    [SerializeField] private string oilStorageSceneName;
    [SerializeField] private string citySceneName;

    public GameObject mainPanel;                // 로비(메인)
    public GameObject stealthMapSelectPanel;    // 스텔스 맵 선택 패널
    public GameObject tdmMapSelectPanel;        // 팀데스매치 맵 선택 패널
    public GameObject weaponSelectPanel;        // 무기고 선택 패널

    public GameObject helpPanel;    // 도움말 패널
    public GameObject settingPanel; // 설정 패널
    public GameObject quitPanel;    // 종료 확인 패널

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
        cityToggle.isOn = false;
    }
    
    // 무기고 열기
    public void OnClickWeaponSelect()
    {
        CloseAllPanel();
        weaponSelectPanel.SetActive(true);
    }
    #endregion

    #region 팝업 창
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
            if (cityToggle.isOn)
            {
                SceneLoading.LoadTo(citySceneName);
                return;
            }
        }
        Debug.Log("맵 선택 안됨"); // 예외 처리
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
