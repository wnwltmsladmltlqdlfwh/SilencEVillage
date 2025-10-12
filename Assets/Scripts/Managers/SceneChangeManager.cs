using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    private SceneType currentSceneType;

    private void Start()
    {
        currentSceneType = SceneType.LobbyScene;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }
    
    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        // 1. 화면을 어둡게 만들기
        yield return StartCoroutine(CameraManager.Instance.FadeToBlack(1f));
        
        // 2. 씬 로드
        SceneManager.LoadScene(sceneName);
        currentSceneType = (SceneType)Enum.Parse(typeof(SceneType), sceneName);
    }
    
    
}