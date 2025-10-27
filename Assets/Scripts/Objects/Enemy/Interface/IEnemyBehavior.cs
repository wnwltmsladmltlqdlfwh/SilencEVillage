public interface IEnemyBehavior
{
    void OnNearByPlayer();
    void OnPlayerDetected();
    void OnAreaEntered(AreaType areaType);
    void OnAreaExited(AreaType areaType);
}