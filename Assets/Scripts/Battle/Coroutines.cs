using Unity.VisualScripting;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Coroutines Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
