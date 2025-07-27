using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FadeInOut : MonoBehaviour
{
    public bool isFadeIn = true;
    
    private CancellationTokenSource disableCancellation = new CancellationTokenSource();

    private void OnEnable()
    {
        if (disableCancellation != null)
        {
            disableCancellation.Dispose();
        }
        disableCancellation = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        disableCancellation.Cancel();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isFadeIn)
        {
            gameObject.SetActive(true);
            UFadeOut().Forget();
        }
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        UFadeIn().Forget();
    }

    private async UniTaskVoid UFadeOut()
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.5f;

        while (elapsedTime < fadeDuration)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            
            await UniTask.Yield();
        }
        
        gameObject.SetActive(false);
    }

    private async UniTaskVoid UFadeIn()
    {
        float elapsedTime = 0f;
        float fadeDuration = 0.5f;

        while (elapsedTime < fadeDuration)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            
            await UniTask.Yield();
        }
        
        gameObject.SetActive(false);
    }
}
