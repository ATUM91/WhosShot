using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// UI에서 설정 값 변경 처리

public class SettingUI : MonoBehaviour
{
    [Header("오디오 UI")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("해상도 UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("화면 모드 UI")]
    [SerializeField] private TMP_Dropdown screenModeDropdown;

    [Header("크로스 헤어")]
    [SerializeField] private Image crosshairPreviewImage;
    [SerializeField] private Sprite[] crosshairSprite;

    void Start()
    {
        InitResolutionDropdown();
        InitScreenModeDropdown();

        // 저장된 번호의 크로스헤어 프리뷰 적용
        crosshairPreviewImage.sprite = crosshairSprite[SettingManager.Instance.crosshairIndex];
    }

    // 해상도 초기화
    private void InitResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();  // 기존 옵션 제거
        List<string> options = new List<string>();  // 드롭다운에 표시할 목록
        Vector2Int[] resolutions = SettingManager.Instance.GetResolutions();

        // 드롭다운 문자열 생성
        for (int i = 0; i < resolutions.Length; i++)
        { 
            string option = resolutions[i].x + "x" + resolutions[i].y;
            options.Add(option);    // 드롭다운 문자열 추가
        }
        resolutionDropdown.AddOptions(options); // 옵션 추가
        resolutionDropdown.value = SettingManager.Instance.resolutionIndex; // 저장된 값 적용
        resolutionDropdown.RefreshShownValue();
    }

    // 화면모드 초기화
    private void InitScreenModeDropdown()
    {
        screenModeDropdown.value = SettingManager.Instance.screenModeIndex;
        screenModeDropdown.RefreshShownValue();
    }

    #region UI 호출 함수
    // 배경음 조절
    public void SetBGMVolume(float value)
    {
        SettingManager.Instance.SetBGMVolume(value); // 즉시 적용
        SettingManager.Instance.SettingSave();
    }

    // 효과음 조절
    public void SetSFXVolume(float value)
    { 
        SettingManager.Instance.SetSFXVolume(value); // 즉시 적용
        SettingManager.Instance.SettingSave();
    }

    // 마우스 감도 조절
    public void SetMouseSensitivity(float value)
    {
        float mValue = Mathf.Clamp(value, 0.1f, 50f);
        SettingManager.Instance.mouseSensitivity = mValue;
        SettingManager.Instance.SettingSave();
    }

    // 밝기 조절
    public void SetBrightness(float value)
    { 
        SettingManager.Instance.brightness = value;
        SettingManager.Instance.ApplyBrightness(); // 즉시 적용
        SettingManager.Instance.SettingSave();
    }

    // 해상도 변경
    public void SetResolution(int index)
    {
        SettingManager.Instance.SetResolution(index);
        SettingManager.Instance.SettingSave();
    }

    // 화면 모드 변경
    public void SetScreenMode(int index)
    { 
        SettingManager.Instance.SetScreenMode(index);
        SettingManager.Instance.SettingSave();
    }

    // 크로스헤어 변경
    public void SetCrosshair(int index)
    {
        SettingManager.Instance.SetCrosshair(index); // 현재 번호 저장
        crosshairPreviewImage.sprite = crosshairSprite[index]; // 프리뷰 이미지 변경

        if (StealthUIManager.Instance != null)
        { 
            StealthUIManager.Instance.ApplyCrosshair();
        }
        SettingManager.Instance.SettingSave();
    }
    #endregion
}
