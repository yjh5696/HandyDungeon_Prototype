using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float fadeDuration = 2f;
    private CancellationTokenSource _disableCancellation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FadeOut().Forget();
    }
    
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

    private async UniTaskVoid FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            text.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            
            await UniTask.Yield(cancellationToken: _disableCancellation.Token);
        }
        
        await UniTask.WaitForSeconds(1.0f);

        FadeOut().Forget();
    }
    
    private async UniTaskVoid FadeOut()
    {
        float elapsedTime = 0f;
        
        await UniTask.WaitForSeconds(1.0f);

        while (elapsedTime < fadeDuration)
        {
            text.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            
            await UniTask.Yield(cancellationToken: _disableCancellation.Token);
        }
        
        FadeIn().Forget();
    }
}
