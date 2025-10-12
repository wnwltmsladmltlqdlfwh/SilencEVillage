using UnityEngine;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<UIType, GameObject> uiList = new Dictionary<UIType, GameObject>();

    public void OpenUI(UIType uiType)
    {
        uiList[uiType].SetActive(true);
    }
    
    
}
