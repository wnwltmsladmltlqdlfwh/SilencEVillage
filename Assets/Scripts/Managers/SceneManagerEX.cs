using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.UI;

public class SceneManagerEX : Singleton<SceneManagerEX>
{
    public SceneType currentScene;
    [SerializeField] private Image fadeImage;
    public Action OnSceneChanged;

    protected override void Awake()
    {
        base.Awake();

        if(Instance == this)
        {
            transform.SetParent(null); 
            DontDestroyOnLoad(this.gameObject);
            CreateFadeImage();
        }
    }

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
        SceneType previousScene = currentScene;
        // 1. 화면을 어둡게 만들기
        yield return StartCoroutine(FadeOutUI(1f));

        // 2. 이전 씬과 현재 씬이 다르면 모든 매니저 초기화
        if (previousScene != currentScene)
        {
            CleanupAllManagers();
        }

        // 3. 씬 로드
        SceneManager.LoadScene(sceneName);
        currentScene = (SceneType)Enum.Parse(typeof(SceneType), sceneName);

        yield return new WaitUntil(() => SceneManager.GetSceneByName(sceneName).isLoaded);

        if(SceneManager.GetSceneByName("LobbyScene").isLoaded)
        {
            SoundManager.Instance.PlayBGM(SoundManager.BGMType.BGM_Lobby);
        }
        else
        {
            SoundManager.Instance.StopBGM();
        }

        yield return StartCoroutine(FadeInUI(1f));
    }

    public void CleanupAllManagers()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StopGameTimer();
            TimeManager.Instance.OnTimeUpdated = null;
            TimeManager.Instance.OnPhaseChanged = null;
            TimeManager.Instance.OnDayNightChanged = null;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged = null;
            GameManager.Instance.OnAreaChanged = null;
            GameManager.Instance.OnGameVictory = null;
            GameManager.Instance.OnGameDefeat = null;
        }
    }

    // 동적으로 FadeImage를 생성하는 함수
    private void CreateFadeImage()
    {
        if (fadeImage != null) return;

        // Canvas가 없으면 생성
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("SceneTransitionCanvas");
            canvasObj.transform.SetParent(transform);
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;  // 최상위
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Fade Image 생성
        GameObject fadeObj = new GameObject("FadeImage");
        fadeObj.transform.SetParent(canvas.transform, false);
        
        fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false;
        
        // 전체 화면 크기
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        Debug.Log("SceneManagerEX: FadeImage 생성 완료");
    }

    public IEnumerator FadeOutUI(float fadeSpeed)
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1f);
        fadeImage.raycastTarget = true;
    }

    public IEnumerator FadeInUI(float fadeSpeed)
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0f);
        fadeImage.raycastTarget = false;
    }
}