using UnityEngine;
using UnityEngine.SceneManagement;

// 타이틀 씬
// 아무 키 입력 시 로딩 씬으로 이동

public class SceneTitle : MonoBehaviour
{
    void Update()
    {
        // 아무 키 입력
        if (Input.anyKeyDown)
        {
            // 로딩씬으로 이동
            SceneLoading.LoadTo("Scene Lobby");
        }
    }
}
