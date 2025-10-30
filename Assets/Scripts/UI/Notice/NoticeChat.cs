using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class NoticeChat : MonoBehaviour
{
    [SerializeField] private TMP_Text noticeText;
    [SerializeField] private TMP_Text timeText;

    private Dictionary<NoticeType, Color> noticeColorMap;

    void Awake()
    {
        InitializeColorsMap();
    }

    private void InitializeColorsMap()
    {
        noticeColorMap = new Dictionary<NoticeType, Color>()
        {
            {NoticeType.Normal, new Color(1f, 1f, 1f, 1f)},     // 하얀색 (Normal)
            {NoticeType.Warning, new Color(1f, 0f, 0f, 1f)},    // 빨간색 (Warning)
            {NoticeType.System, new Color(0f, 1f, 0f, 1f)},     // 초록색 (System)
        };
    }

    public void SetNotice(string message, NoticeType noticeType)
    {
        noticeText.text = message;
        int minutes = Mathf.FloorToInt(TimeManager.Instance.CurrentTime / 60);
        int seconds = Mathf.FloorToInt(TimeManager.Instance.CurrentTime % 60);
        timeText.text = $"{minutes:00} : {seconds:00}";
        noticeText.color = noticeColorMap[noticeType];
    }
}
