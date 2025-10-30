using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnClickOptionButton);
    }

    void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClickOptionButton);
    }

    private void OnClickOptionButton()
    {
        OptionManager.Instance.ShowOptionPanel();
    }
}
