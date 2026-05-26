using UnityEngine;

// 미션 목표물

public class MissionTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MissionManager.Instance.SetPlayerInRange(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MissionManager.Instance.SetPlayerInRange(false);
        }
    }
}
