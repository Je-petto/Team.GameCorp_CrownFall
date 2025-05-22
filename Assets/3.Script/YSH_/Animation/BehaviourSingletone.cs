using UnityEngine;

//template : 틀, 형 -> 사용법: <T>
// 싱글톤 : 관리자 , 전역적으로 사용, 유일한 존재로 만들기 위해 사용
// BehaviourSingleton : 런타임(실행중)에서만 존재, Editor에서도 존재하기 위해서는 ScriptableSingleton사용
public abstract class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _instance;

    //외부에서 부를 때
    public static T I
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    
                    GameObject o = new GameObject(typeof(T).Name);
                    _instance = o.AddComponent<T>();

                }
            }
            return _instance;
        }
        //set {} - 사용하지 않음
    }

    protected abstract bool IsDontdestroy();

    protected virtual void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        
        if (IsDontdestroy())
            DontDestroyOnLoad(gameObject);

    }

}
