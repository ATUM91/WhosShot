using UnityEngine;

// UI에서 설정 값 변경 처리

public class SettingUI : MonoBehaviour
{
    // 마우스 감도 조절
    public void SetMouseSensitivity(float value)
    {
        SettingManager.Instance.mouseSensitivity = value;
    }

    // 전체 볼륨 조절
    public void SetMasterVolume(float value)
    { 
        SettingManager.Instance.masterVolume = value;
        SettingManager.Instance.ApplySetting(); // 즉시 적용
    }

    // 배경음 조절
    public void SetBGMVolume(float value)
    { 
        SettingManager.Instance.bgmVolume = value;
    }

    // 효과음 조절
    public void SetSFXVolume(float value)
    { 
        SettingManager.Instance.sfxVolume = value;
    }

    // 밝기 조절
    public void SetBrightness(float value)
    { 
        SettingManager.Instance.brightness = value;
        SettingManager.Instance.ApplySetting(); // 즉시 적용
    }

    // 크로스헤어 변경
    public void SetCrosshair(int index)
    { 
        SettingManager.Instance.crosshairIndex = index;
    }

    // 저장 버튼
    public void OnClickSave()
    { 
        SettingManager.Instance.SettingSave();
    }
}
