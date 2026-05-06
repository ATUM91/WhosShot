using UnityEngine;

// 게임 설정 값 저장 / 불러오기
// PlayerPrefs를 사용해 저장
// 게임 시작 시 자동 적용

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance; // 싱글톤 인스턴스

    [Header("선택된 BGM")]
    [SerializeField] public int bgmIndex = 0;

    [Header("오디오 볼륨")]
    [SerializeField] public float bgmVolume = 1f;      // 배경음
    [SerializeField] public float sfxVolume = 1f;      // 효과음

    [Header("마우스 감도")]
    [SerializeField] public float mouseSensitivity = 200f; // 마우스 감도

    [Header("화면 설정")]
    [SerializeField] public float brightness = 1f;      // 밝기 조절
    [SerializeField] public int displayIndex = 0;       // 해상도 선택
    [SerializeField] public int screenModeIndex = 0;    // 화면 모드 0:전체, 1:전체창, 2:창

    [Header("크로스헤어")]
    [SerializeField] public int crosshairIndex = 0;    // 크로스헤어 종류

    private void Awake()
    {
        if (Instance != null && Instance != this) // 싱글톤 설정
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 이동시 유지

        SettingLoad();  // 세팅 저장 값 불러오기
        ApplySetting(); // 전체 설정 적용
    }

    // 설정 값 적용 / 그래픽 관련
    public void ApplySetting()
    {
        ApplyBrightness();  // 밝기 적용
        ApplyDisplay();     // 해상도 적용
        ApplyScreenMode();  // 화면 모드 적용
    }

    // 밝기 조절
    public void ApplyBrightness()
    {
        // 밝기 적용
        RenderSettings.ambientLight = Color.white * brightness;
    }

    // 해상도 선택 / OS나 그래픽카드가 지원하는 해상도 목록을 가져옴 / 하드웨어 종속적인 동적 데이터라 배열 사용
    public void ApplyDisplay()
    {
        Resolution[] resolutions = Screen.resolutions;
        if (displayIndex < 0 || displayIndex >= resolutions.Length)
        { 
            displayIndex = resolutions.Length - 1;
        }
        Resolution resolution = resolutions[displayIndex];
        // 현재 화면 모드 유지
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    // 화면 모드 선택 / 엔진에서 정의된 고정 상태 값이라 switch문 사용
    public void ApplyScreenMode()
    {
        FullScreenMode screenMode = FullScreenMode.ExclusiveFullScreen;

        switch (screenModeIndex)
        {
            case 0: 
                screenMode = FullScreenMode.ExclusiveFullScreen; // 전체 화면
                break;
            case 1:
                screenMode = FullScreenMode.FullScreenWindow; // 전체 창모드
                break;
            case 2:
                screenMode = FullScreenMode.Windowed; // 창모드
                break;
        }
        // 현재 해상도 유지
        Screen.SetResolution(Screen.width, Screen.height, screenMode);
    }

    // BGM 볼륨 조절
    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        // 사운드 매니저가 존재하면 반영
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.RefreshVolume(bgmVolume, sfxVolume);
        }
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        // 사운드 매니저가 존재하면 반영
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.RefreshVolume(bgmVolume, sfxVolume);
        }
    }

    // 설정 저장
    public void SettingSave()
    {
        PlayerPrefs.SetFloat("Mouse", mouseSensitivity);    // 마우스 감도
        PlayerPrefs.SetFloat("BGM", bgmVolume);             // BGM 볼륨
        PlayerPrefs.SetFloat("SFX", sfxVolume);             // SFX 볼륨
        PlayerPrefs.SetFloat("Brightness", brightness);     // 밝기
        PlayerPrefs.SetInt("Display", displayIndex);        // 해상도
        PlayerPrefs.SetInt("ScreenMode", screenModeIndex);  // 화면 모드
        PlayerPrefs.SetInt("Crosshair", crosshairIndex);    // 크로스헤어
        PlayerPrefs.SetInt("BGM_Index", bgmIndex);          // 선택된 BGM

        PlayerPrefs.Save(); // 저장
    }

    // 설정 불러오기
    public void SettingLoad()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("Mouse", 200f);
        bgmVolume = PlayerPrefs.GetFloat("BGM", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);
        brightness = PlayerPrefs.GetFloat("Brightness", 1f);
        displayIndex = PlayerPrefs.GetInt("Display", 0);
        screenModeIndex = PlayerPrefs.GetInt("ScreenMode", 0);
        crosshairIndex = PlayerPrefs.GetInt("Crosshair", 0);
        bgmIndex = PlayerPrefs.GetInt("BGM_Index", 0);
    }
}
