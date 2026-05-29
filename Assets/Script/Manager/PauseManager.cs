using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool isPause = false;

    [Header("참조")]
    [SerializeField] private GameObject pauseUI; // 기본 Pause 메뉴 UI
    [SerializeField] private GameObject settingPanel; // 설정 패널 재사용

    void Update()
    {
        InputPause();
    }

    private void InputPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                Resume();
            }
            else
            { 
                Pause();
            }
        }
    }

    // 게임 정지
    public void Pause()
    {
        isPause = true;
        Time.timeScale = 0f;

        pauseUI.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 계속하기
    public void Resume()
    {
        isPause = false;
        Time.timeScale = 1f;

        pauseUI.SetActive(false);

        // 설정창 같이 종료 / 중복 방지
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 설정창 열기
    public void OpenSetting()
    {
        pauseUI.SetActive(false);
        settingPanel.SetActive(true);
    }

    // 설정창 닫기
    public void CloseSetting()
    {
        settingPanel.SetActive(false);
        pauseUI.SetActive(true);
    }

    // 로비로 나가기
    public void GoLobby()
    { 
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene Lobby");
    }

    // 게임 종료
    public void QuitGame()
    { 
        Application.Quit();
    }
}
