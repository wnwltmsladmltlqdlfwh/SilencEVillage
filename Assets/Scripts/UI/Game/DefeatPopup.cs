using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DefeatPopup : MonoBehaviour
{
    [SerializeField] private Image enemyImage;
    [SerializeField] private TextMeshProUGUI enemyKilledPlayerText;
    [SerializeField] private TextMeshProUGUI defeatTimeText;
    private Dictionary<EnemyType, string> enemyKilledPlayerDictionary = new Dictionary<EnemyType, string>()
    {
        { EnemyType.Jangsanbeom, "장산범은 새로운 목소리를 얻었습니다." },
        { EnemyType.Seongju, "성주는 마을을 지켜냈습니다." },
        { EnemyType.Jibakryeong, "지박령에겐 동료가 생겼습니다." },
        { EnemyType.Agwi, "아귀는 주린 배를 채울 수 있었습니다." },
    };

    public void SetupInfoTheEnemy(EnemyDataSO enemyData)
    {
        enemyImage.sprite = enemyData.EnemyImage;
        enemyKilledPlayerText.text = enemyKilledPlayerDictionary[enemyData.EnemyType];
        
        int dayCount = TimeManager.Instance.CurrentPhase;
        int timeCount = Mathf.FloorToInt(TimeManager.Instance.CurrentTime);
        string dayLight = TimeManager.Instance.IsDayTime ? "오전" : "오후";
        defeatTimeText.text = $"{dayCount}일 {dayLight} {timeCount / 60:00}:{timeCount % 60:00} 사망";
    }
}
