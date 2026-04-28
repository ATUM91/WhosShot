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
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        ApplyVolume(); // 실시간 볼륨 적용
    }

    // 볼륨 적용
    private void ApplyVolume()
    { 
        // SettingManager 인스턴스 가져오기
        SettingManager setting = SettingManager.Instance;
        // 전체 볼륨 * 개별 볼륨
        bgmSource.volume = setting.masterVolume * setting.bgmVolume;
        sfxSource.volume = setting.masterVolume * setting.sfxVolume;
    }

    // SFX 재생 함수
    public void PlaySFX(AudioClip audioClip)
    { 
        sfxSource.PlayOneShot(audioClip);
    }
}
