using Sirenix.OdinInspector;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Object
{
    [SerializeField, ToggleLeft] protected bool dontDestroyOnLoad = true;

    static T instance;
    public static T Instance {
        get {
            if (instance) return instance;
            
            instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (instance) return instance;
            
            GameObject go = new (typeof(T).Name, typeof(T));
            if (Application.isPlaying) DontDestroyOnLoad(go);                        
            return instance;
        }
    }

    protected virtual void Awake() {
        if (!instance)
            instance = this as T;
        else if (instance != this as T) 
            Destroy(gameObject);
        
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }
}