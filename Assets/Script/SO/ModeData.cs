using UnityEngine;

// 게임 모드 SO
// 스텔스모드 / 팀데스매치(TDM)모드

[CreateAssetMenu(menuName = "Game/GameMode")]
public class ModeData : ScriptableObject
{
    [Header("스텔스 모드")]
    public int stealthEnemyCount;
    public int stealthMaxkillCount;

    [Header("팀 데스매치 모드")]
    public int tdmKillCount;

    public string modeName;

    public bool useRandomSpawn;
}
