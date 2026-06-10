using UnityEngine;

// 애니메이션 기반 문 기믹

public class GimmickDoor : MonoBehaviour
{
    [Header("문")]
    [SerializeField] private Animator animator;

    private bool isOpen;

    public bool IsOpen()
    {
        return isOpen;
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
    }
}
