using TMPro;
using UnityEngine;

// 인게임 미션 텍스트 출력

public class MissionUI : MonoBehaviour
{
    public static MissionUI Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text missionText;

    private int enemyKillCount = 0;
    private int enemyKillLimit = 3;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        UpdateUI();
    }

    // 외부 호출 / 킬카운트 증가
    public void AddKill()
    { 
        enemyKillCount++;
        UpdateUI();
    }

    public void UpdateUI()
    {
        missionText.text =
            "미션 성공 조건\n" +
            "C4 회수\n" +
            "\n" +
            "미션 실패 조건\n" +
            "플레이어 사망\n" +
            "4명 이상 사살\n" +
            $"적 사살 가능 수 ( {enemyKillCount} / {enemyKillLimit} )";
    }
}
