using UnityEngine;

// 발각 수치가 가장 높은 것만 저장

public class DetectManager : MonoBehaviour
{
    public static DetectManager Instance;

    private float currentDetect;
    private float maxDetect = 100f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        currentDetect = 0f;
    }

    // 발각 수치 전달
    public void UpdateDetect(float detectValue)
    {
        if (detectValue > currentDetect)
        { 
            currentDetect = detectValue;
        }
    }

    public float GetDetect()
    {
        return currentDetect;
    }

    public float GetMaxDetect()
    { 
        return maxDetect;
    }
}
