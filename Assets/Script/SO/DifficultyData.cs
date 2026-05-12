using UnityEngine;

// 난이도 SO
// 스텔스모드 - 적 AI 개체 수 증가
// 팀 데스매치 모드 - 적 AI 사격 반응속도 / 정확도 증가

[CreateAssetMenu(menuName = "Game/Difficulty")]
public class DifficultyData : ScriptableObject
{
    [Header("스텔스 AI")]
    public int addStealthEnemyCount;
    public int addStealthMaxkillCount;

    [Header("팀 데스매치 AI")]
    public float tdmAiReactionTime;
    public float tdmAiHitRate;

    public string difficultyName;
}
