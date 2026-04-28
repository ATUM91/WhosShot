using UnityEngine;

// 게임 설정 값 저장 / 불러오기

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    [Header("마우스 감도")]
    [SerializeField] public float mouseSensitivity = 200f; // 마우스 감도

    [Header("오디오 볼륨")]
    [SerializeField] public float masterVolume = 1f;   // 전체 볼륨
    [SerializeField] public float bgmVolume = 1f;      // 배경음
    [SerializeField] public float sfxVolume = 1f;      // 효과음

    [Header("밝기 조절")]
    [SerializeField] public float brightness = 1f;     // 밝기

    [Header("크로스헤어")]
    [SerializeField] public int crosshairIndex = 0;    // 크로스헤어 종류

    private void Awake()
    {
        if (Instance == null) // 싱글톤 설정
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // 씬 이동시 유지
        SettingLoad();  // 세팅 저장 값 불러오기
        ApplySetting(); // 적용
    }

    // 설정 값 적용
    public void ApplySetting()
    { 
        // 전체 볼륨 적용
        AudioListener.volume = masterVolume;
        // 밝기 적용
        RenderSettings.ambientLight = Color.white * brightness;
    }

    // 설정 저장
    public void SettingSave()
    {
        PlayerPrefs.SetFloat("Mouse", mouseSensitivity);
        PlayerPrefs.SetFloat("Master", masterVolume);
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
        masterVolume = PlayerPrefs.GetFloat("Master", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGM", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);
        brightness = PlayerPrefs.GetFloat("Brightness", 1f);
        crosshairIndex = PlayerPrefs.GetInt("Crosshair", 0);
    }
}
