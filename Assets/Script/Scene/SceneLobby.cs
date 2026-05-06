using UnityEngine;

// 모드 선택 -> 해당 씬으로 이동

public class SceneLobby : MonoBehaviour
{
    // 스텔스(Stealth) 모드 // 유류 저장소
    public void StartStealth()
    {
        SceneLoading.LoadTo("Stealth_Oil Storage");
    }

    // 팀데스매치(TDM) 모드 / 보급 센터
    public void StartTDM()
    {
        SceneLoading.LoadTo("TDM_Supply Center");
    }
}
