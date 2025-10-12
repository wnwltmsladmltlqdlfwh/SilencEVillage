using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TextMeshProButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Button Settings")]
    [SerializeField] private float pressedAlpha = 0.5f;
    [SerializeField] private float normalAlpha = 1f;
    
    [Header("Events")]
    [SerializeField] private UnityEngine.Events.UnityEvent onReleasedEvent;
    
    private TextMeshProUGUI textMeshPro;
    private bool isPressed = false;
    
    // C# 이벤트 (코드에서 구독 가능)
    public event Action OnButtonReleased;
    
    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null)
            return;
    }
    
    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (textMeshPro != null)
        {
            isPressed = true;
            textMeshPro.alpha = pressedAlpha;
        }
    }
    
    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (isPressed)
        {
            ReleaseButton();
        }
    }
    
    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isPressed = false;
    }
    
    private void ReleaseButton()
    {
        if (textMeshPro != null)
        {
            textMeshPro.alpha = normalAlpha;
        }
        
        isPressed = false;
        
        // UnityEvent 실행
        onReleasedEvent?.Invoke();
        
        // C# 이벤트 실행
        OnButtonReleased?.Invoke();
    }
    
    // 외부에서 이벤트 등록하는 메서드
    public void AddOnReleasedListener(Action action)
    {
        OnButtonReleased += action;
    }
    
    public void RemoveOnReleasedListener(Action action)
    {
        OnButtonReleased -= action;
    }
}
