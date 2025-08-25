using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FadeInOut : MonoBehaviour
{
    public bool isFading = false;
    
    private CancellationTokenSource _disableCancellation;

    private void OnEnable()
    {
        _disableCancellation?.Dispose();
        _disableCancellation = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _disableCancellation.Cancel();
    }

    private void OnDestroy()
    {
        _disableCancellation.Cancel();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _disableCancellation = new CancellationTokenSource();
    }

    public void FadeIn(float duration)
    {
        UFadeIn(duration).Forget();
    }
    
    public void FadeOut(float duration)
    {
        UFadeOut(duration).Forget();
    }

    private async UniTaskVoid UFadeOut(float duration)
    {
        float elapsedTime = 0f;
        float fadeDuration = duration;
        
        isFading = true;

        while (elapsedTime < fadeDuration)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            
            await UniTask.Yield(cancellationToken: _disableCancellation.Token);
        }
        
        isFading = false;
        gameObject.SetActive(false);
    }

    private async UniTaskVoid UFadeIn(float duration)
    {
        float elapsedTime = 0f;
        float fadeDuration = duration;
        
        isFading = true;

        while (elapsedTime < fadeDuration)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            
            await UniTask.Yield(cancellationToken: _disableCancellation.Token);
        }
        
        isFading = false;
    }
}
