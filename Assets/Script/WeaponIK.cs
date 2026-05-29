using UnityEngine;

// 총에 왼손 고정

public class WeaponIK : MonoBehaviour
{
    [Header("적 애니메이터")]
    [SerializeField] private Animator animator;

    private Transform leftHandTarget;

    public void SetLeftHandTarget(Transform target)
    { 
        leftHandTarget = target;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;
        if (leftHandTarget == null) return;

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, -0.4f);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
    }
}
