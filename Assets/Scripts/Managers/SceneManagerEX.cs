using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneManagerEX : Singleton<SceneManagerEX>
{
    public SceneType currentScene;

    public Action OnSceneChanged;

    private void Start()
    {
        currentScene = SceneType.LobbyScene;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName)); 
    }
    
    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        // 1. 화면을 어둡게 만들기
        yield return StartCoroutine(UIManager.Instance.FadeOutUI(1f));
        
        // 2. 씬 로드
        SceneManager.LoadScene(sceneName);
        currentScene = (SceneType)Enum.Parse(typeof(SceneType), sceneName);
    }
}