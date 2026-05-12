using UnityEngine;
using UnityEngine.UI;

// 시체 하이라이트 처리

public class DeadBodyHighlight : MonoBehaviour
{
    [Header("아웃 라인")]
    private Outline outline;

    private bool onDeadBody;

    void Awake()
    {
        outline = GetComponentInChildren<Outline>();
        if (outline != null)
        { 
            outline.enabled = false;
        }
    }

    // 시체 상태 활성화
    public void OnDeadBody()
    { 
        onDeadBody = true;
        // 죽기전에 아웃라인 안보이도록
        if (outline != null)
        { 
            outline.enabled = false;
        }
    }

    // 하이라이트 On / Off
    public void SetHighlight(bool isHighlight)
    {
        if (!onDeadBody) return;
        if (outline == null) return;

        outline.enabled = isHighlight;
    }
}
