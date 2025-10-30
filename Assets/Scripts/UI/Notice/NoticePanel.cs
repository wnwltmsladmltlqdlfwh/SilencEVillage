using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NoticePanel : MonoBehaviour
{
    [SerializeField] private Transform noticePanelParent;
    [SerializeField] private ScrollRect noticeScrollRect;
    [SerializeField] private NoticeChat noticeChatPrefab;
    [SerializeField] private int poolSize = 30; // 초기 풀 크기
    [SerializeField] private int maxVisibleNotices = 40; // 최대 표시 가능한 알림 개수

    private Queue<NoticeChat> noticePool;   // 초기 풀 관리
    private List<NoticeChat> activeNotices; // 현재 활성화 중인 알림 목록

    void Awake()
    {
        InitializePool();
    }

    private void Start()
    {
        UIManager.Instance.OnNoticeAdded += AddNotice;
    }
    
    private void OnDestroy()
    {
        UIManager.Instance.OnNoticeAdded -= AddNotice;
    }

    private void InitializePool()
    {
        noticePool = new Queue<NoticeChat>();
        activeNotices = new List<NoticeChat>();
        for (int i = 0; i < poolSize; i++)
        {
            NoticeChat noticeChat = Instantiate(noticeChatPrefab, noticePanelParent);
            noticeChat.gameObject.SetActive(false);
            noticePool.Enqueue(noticeChat);
        }
    }

    public void AddNotice(string message, NoticeType noticeType)
    {
        NoticeChat newNotice = GetNoticeFromPool();
        if(newNotice != null)
        {
            newNotice.SetNotice(message, noticeType);
            newNotice.gameObject.SetActive(true);
            newNotice.transform.SetAsLastSibling(); // 최신 알림을 맨 뒤에 추가

            activeNotices.Add(newNotice);

            if(activeNotices.Count > maxVisibleNotices)
            {
                NoticeChat oldestNotice = activeNotices[0];
                ReturnNoticeToPool(oldestNotice);
                activeNotices.RemoveAt(0);
            }
        }

        ScrollToBottom();   // 자동 스크롤
    }

    private NoticeChat GetNoticeFromPool()
    {
        if (noticePool.Count > 0)
        {
            return noticePool.Dequeue(); ;
        }
        else
        {
            NoticeChat newNotice = Instantiate(noticeChatPrefab, noticePanelParent);
            return newNotice;
        }
    }

    private void ReturnNoticeToPool(NoticeChat notice)
    {
        notice.gameObject.SetActive(false);
        noticePool.Enqueue(notice);
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        noticeScrollRect.verticalNormalizedPosition = 0f; // 스크롤을 맨 아래로 이동
    }
}