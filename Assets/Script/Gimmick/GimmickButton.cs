using UnityEngine;

// 애니메이션 기반 버튼 기믹


public class GimmickButton : MonoBehaviour
{
    [Header("연결된 문")]
    [SerializeField] private GimmickDoor door;

    [Header("버튼 애니메이터")]
    [SerializeField] private Animator animator;

    private bool isPress;

    public void Press()
    {
        if (isPress) return;

        isPress = true;

        if (animator != null)
        {
            animator.SetTrigger("Press");
        }

        if (door != null)
        { 
            door.OpenDoor();
        }
    }

    public GimmickDoor GetDoor()
    {
        return door;
    }
}
