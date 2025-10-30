using UnityEngine;

public class JangsanbeomBehavior : IEnemyBehavior
{
    private EnemyDataSO data;
    private bool hasPlayedSound = false;
    private SoundManager soundManager;

    public JangsanbeomBehavior(EnemyDataSO enemyData)
    {
        this.data = enemyData;
        soundManager = SoundManager.Instance;
    }

    public void OnAreaEntered(AreaType areaType)
    {
        // 지역 진입 시 로직
        Debug.Log($"{data.EnemyName}이 {areaType} 지역에 진입했습니다.");
        if (EnemyManager.Instance.IsNearByPlayer(data.EnemyType) || areaType == AreaManager.Instance.PlayerCurrentArea.AreaType)
        {
            if (!hasPlayedSound)
            {
                soundManager.PlayVoice_Jangsanbeom();
                hasPlayedSound = true;
            }
        }
        else if(hasPlayedSound)
        {
            hasPlayedSound = false;
        }
    }

    public void OnAreaExited(AreaType areaType)
    {
        // 지역 이탈 시 로직
        Debug.Log($"{data.EnemyName}이 {areaType} 지역에서 이탈했습니다.");
    }
}