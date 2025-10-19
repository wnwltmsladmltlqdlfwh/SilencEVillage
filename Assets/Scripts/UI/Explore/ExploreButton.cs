using UnityEngine;

public class ExploreButton : MonoBehaviour
{
    // 지연 초기화를 위한 매니저 참조
    private AreaManager _areaManager;
    private UIManager _uiManager;
    
    // 프로퍼티로 안전한 접근
    private AreaManager areaManager => _areaManager != null ? _areaManager : (_areaManager = AreaManager.Instance);
    private UIManager uiManager => _uiManager != null ? _uiManager : (_uiManager = UIManager.Instance);

    public void OnClickExploreButton()
    {
        uiManager.PlayerMovement();
    }
}
