using UnityEngine;

// 게임 설정 값 저장 / 불러오기

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    [Header("오디오 볼륨")]
    [SerializeField] public float bgmVolume = 1f;      // 배경음
    [SerializeField] public float sfxVolume = 1f;      // 효과음

    [Header("마우스 감도")]
    [SerializeField] public float mouseSensitivity = 200f; // 마우스 감도

    [Header("밝기 조절")]
    [SerializeField] public float brightness = 1f;     // 밝기

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
        ApplySetting(); // 적용
    }

    // 설정 값 적용
    public void ApplySetting()
    {
        // 밝기 적용
        RenderSettings.ambientLight = Color.white * brightness;
    }

    // 설정 저장
    public void SettingSave()
    {
        PlayerPrefs.SetFloat("Mouse", mouseSensitivity);
        PlayerPrefs.SetFloat("BGM", bgmVolume);
        PlayerPrefs.SetFloat("SFX", sfxVolume);
        PlayerPrefs.SetFloat("Brightness", brightness);
        PlayerPrefs.SetInt("Crosshair", crosshairIndex);

        PlayerPrefs.Save(); // 저장
    }

    // 설정 불러오기
    public void SettingLoad()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("Mouse", 200f);
        bgmVolume = PlayerPrefs.GetFloat("BGM", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);
        brightness = PlayerPrefs.GetFloat("Brightness", 1f);
        crosshairIndex = PlayerPrefs.GetInt("Crosshair", 0);
    }
}
