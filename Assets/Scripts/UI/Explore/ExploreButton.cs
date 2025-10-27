using System.Collections;
using UnityEngine;

public class ExploreButton : MonoBehaviour
{
    // 지연 초기화를 위한 매니저 참조
    private UIManager _uiManager;
    
    // 프로퍼티로 안전한 접근
    private UIManager uiManager => _uiManager != null ? _uiManager : (_uiManager = UIManager.Instance);

    public void OnClickExploreButton()
    {
        if(uiManager.IsMoving || uiManager.IsProgress) return;

        _ = StartCoroutine(ExploreCoroutine());
    }

    private IEnumerator ExploreCoroutine()
    {
        Debug.Log("탐색 시작");
        uiManager.PlayerMovement();
        yield return new WaitWhile(() => uiManager.IsMoving);
        TryGetItem();
    }

    private void TryGetItem()
    {
        float randomValue = Random.Range(0f, 1f);

        if(randomValue < 0.5f)
        {
            ItemManager.Instance.AddItem(ItemManager.Instance.GetRandomItemData());
            Debug.Log("Item 획득");
            return;
        }

        Debug.Log("아이템 획득 실패");
    }
}
