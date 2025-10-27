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
    
    public void OnNearByPlayer()
    {
        if (!hasPlayedSound)
        {
            PlaySound();
            hasPlayedSound = true;
        }
    }
    
    public void OnPlayerDetected()
    {
        // 플레이어 감지 시 추가 로직
        Debug.Log($"{data.EnemyName}이 플레이어를 감지했습니다!");
    }
    
    public void OnAreaEntered(AreaType areaType)
    {
        // 지역 진입 시 로직
        Debug.Log($"{data.EnemyName}이 {areaType} 지역에 진입했습니다.");
    }
    
    public void OnAreaExited(AreaType areaType)
    {
        // 지역 이탈 시 로직
        Debug.Log($"{data.EnemyName}이 {areaType} 지역에서 이탈했습니다.");
    }
    
    private void PlaySound()
    {
        soundManager.PlayVoice_Jangsanbeom();
    }   
}