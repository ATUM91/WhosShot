using UnityEngine;
using UnityEngine.Audio;

// BGM / SFX 관리
// SettingManager 볼륨 값 적용

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 믹서")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("오디오 소스")]
    [SerializeField] private AudioSource bgmSource; // 배경음
    [SerializeField] private AudioSource sfxSource; // 효과음

    [Header("BGM 목록")]
    [SerializeField] private AudioClip[] bgmList;   // 배경음 목록

    // 볼륨 저장용
    private float currentBGMVolume = 1f;
    private float currentSFXVolume = 1f;

    void Awake()
    {
        // 싱글톤 설정 / 중복 생성 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SettingManager settingManager = SettingManager.Instance;
        if (settingManager == null) return;
        RefreshVolume(settingManager.bgmVolume, settingManager.sfxVolume);
        PlayBGM();
    }

    // 볼륨 갱신
    public void RefreshVolume(float bgm, float sfx)
    {
        currentBGMVolume = bgm;
        currentSFXVolume = sfx;

        bgmSource.volume = currentBGMVolume;
        sfxSource.volume = currentSFXVolume;
    }

    // BGM 재생 함수
    public void PlayBGM()
    {
        if (SettingManager.Instance == null) return;
        if (bgmList == null || bgmList.Length == 0) return;

        int index = SettingManager.Instance.bgmIndex;

        if (index < 0 || index >= bgmList.Length)
        { 
            index = 0;
        }

        if (bgmSource.clip == bgmList[index]) return;

        bgmSource.clip = bgmList[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // SFX 재생 함수
    public void PlaySFX(AudioClip audioClip)
    {
        if (audioClip == null) return;
        sfxSource.PlayOneShot(audioClip, currentSFXVolume);
    }
}
