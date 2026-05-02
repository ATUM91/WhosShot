using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// 로딩 연출
// 전달받은 씬으로 이동

public class SceneLoading : MonoBehaviour
{
    private static string targetScene;  // 이동할 씬 이름
    private float loadingTime = 2f;     // 로딩 연출 대기 시간

    // 외부에서 호출
    public static void LoadTo(string sceneName)
    { 
        targetScene = sceneName;

        // 로딩씬으로 이동
        SceneManager.LoadScene("Scene Loading");
    }

    void Start()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        // 로딩 연출중 딜레이 
        yield return new WaitForSeconds(loadingTime);
        // 목표한 최종 씬 이동
        SceneManager.LoadScene(targetScene);
    }
}
