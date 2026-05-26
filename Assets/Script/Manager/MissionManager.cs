using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("미션 상태")]
    private bool playerInRange;
    private bool missionCompleted;
    private bool missionFailed;

    [Header("킬 제한")]
    [SerializeField] private int maxKill = 3;
    private int killCount;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (missionCompleted || missionFailed) return;

        if (killCount > maxKill)
        {
            FailMission();
        }
    }

    public void SetPlayerInRange(bool value)
    {
        playerInRange = value;
    }

    public bool CanInteract()
    {
        return playerInRange && !missionCompleted && !missionFailed;
    }

    public void CompleteMission()
    {
        if (missionCompleted) return;

        missionCompleted = true;
        StealthUIManager.Instance.ShowSuccess();
    }

    public void FailMission()
    {
        if (missionFailed) return;

        missionFailed = true;
        StealthUIManager.Instance.ShowFail();
    }

    public void AddKill()
    {
        killCount++;
    }

    public void SetPlayerDead()
    {
        FailMission();
    }
}