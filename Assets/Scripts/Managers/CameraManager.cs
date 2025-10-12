using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CameraEventType
{
    FadeOut,
    FadeIn,
    Pressed_Middle,
    Shake_Walk,
    Shake_Die,
    MaxCount,
}

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Image fadeImage;

    private Camera mainCamera;

    private void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    public IEnumerator FadeToBlack(float fadeSpeed)
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
}
