using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                
                if (instance == null)
                {
                    var ob = new GameObject(typeof(T).Name);
                    instance = ob.AddComponent<T>();
                }
            }
            
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            
            // DontDestroyOnLoad를 적용하기 전에 루트 오브젝트인지 확인
            if (transform.parent != null)
            {
                // 자식 오브젝트라면 부모에서 분리
                transform.SetParent(null);
            }
            
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}   
