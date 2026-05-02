using UnityEngine;

// 로비 UI 관리
// 각 버튼 클릭에 따라 패널 전환 처리

public class LobbyUI : MonoBehaviour
{
    
    public GameObject mainPanel;                // 로비(메인)
    
    public GameObject stealthMapSelectPanel;    // 스텔스 맵 선택 판넬
    public GameObject tdmMapSelectPanel;        // 팀데스매치 맵 선택 판넬
    
    public GameObject weaponSelectPanel;        // 무기고 선택 판넬

    public GameObject helpPanel;    // 도움말 판넬
    public GameObject settingPanel; // 설정 판넬
    public GameObject quitPanel;    // 종료 확인 판넬

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
    }
    
    // 팀데스매치 모드 선택
    public void OnClickTDMSelect()
    {
        CloseAllPanel();
        tdmMapSelectPanel.SetActive(true);
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

    // 게임 종료
    public void OnClickQuitGame()
    {
        Application.Quit();
    }
    #endregion
}
