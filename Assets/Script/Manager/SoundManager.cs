using UnityEngine;

// BGM / SFX 관리
// SettingManager 볼륨 값 적용

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 소스")]
    [SerializeField] private AudioSource bgmSource; // 배경음
    [SerializeField] private AudioSource sfxSource; // 효과음

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
        ApplyVolume();
    }

    // 볼륨 적용
    private void ApplyVolume()
    { 
        // SettingManager 인스턴스 가져오기
        SettingManager setting = SettingManager.Instance;
        // 초기화 순서 꼬일 때를 방지
        if (setting == null) return;
        // BGM 볼륨
        bgmSource.volume = setting.bgmVolume;
    }

    // SFX 재생 함수
    public void PlaySFX(AudioClip audioClip)
    { 
        sfxSource.PlayOneShot(audioClip);
    }
}
