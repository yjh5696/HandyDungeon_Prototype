using UnityEngine;
using Cysharp.Threading.Tasks;

public class FadeInOut : MonoBehaviour
{
    public bool isFadeIn;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isFadeIn)
        {
            gameObject.SetActive(true);
            
        }
    }

    private async UniTaskVoid FadeIn()
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.5f;

        while (elapsedTime < fadeDuration)
        {
            gameObject.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
        }
    }
}
