using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;
    public bool isLogging = false;
    public bool isExpandable = true;
    private CancellationTokenSource _cancelTokenSource;
    private bool _isExpanded = false;
    private ScrollRect _scrollRect;
    
    
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        _scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        _cancelTokenSource = new CancellationTokenSource();

        text.text = "";
    }

    public void CancelLog()
    {
        _cancelTokenSource.Cancel();
        _cancelTokenSource.Dispose();

        isLogging = false;
        _cancelTokenSource = new CancellationTokenSource(); //토큰으로 비동기 작업 취소하면 계속 취소된 상태로 남아있어서 재생성
    }

    public void AddLog(string msg) //로그 추가하기
    {
        if (text)
            text.text += "\n" + msg;
    }

    public async UniTaskVoid AddDelayedLog(string msg, float delay)
    {
        await UniTask.WaitUntil(() => !isLogging);

        isLogging = true;
        AddLog(msg);
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _cancelTokenSource.Token);
        isLogging = false;
    }

    public void AddSpacingLine()
    {
        //if (text)
            //text.text += "\n----------------------------------\n";
    }

    public async UniTaskVoid StartLog(string str)
    {
        await UniTask.WaitUntil(() => !isLogging);

        isLogging = true;
        string[] lines = str.Split('{');
        foreach (string line in lines)
        {
            if (_cancelTokenSource.Token.IsCancellationRequested) //취소 요청이 들어오면 로그 종료
            {
                isLogging = false;
                break;
            }

            string log = "";
            string wt = "";
            for (int index = 0; index < line.Length; index++)
            {
                char c = line[index];
                if (c == '}')
                {
                    log = line.Remove(0, index + 1);
                    break;
                }

                wt += c;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(double.TryParse(wt, out double t) ? t : 0),
                cancellationToken: _cancelTokenSource.Token);
            AddLog(log);
        }

        AddSpacingLine();
        isLogging = false;
    }

    public async UniTaskVoid PrintScript()
    {
        List<string> strs = new List<string>();
        List<float> f = new List<float>();
        
        foreach (StartScript script in GameManager.Instance.startScripts[GameManager.Instance.currentChapter]
                     .StartScripts)
        {
            if (script.eventID == GameManager.Instance.currentStage)
            {
                strs.Add(script.scriptText);
                f.Add(script.delayTime);
            }
        }

        if (strs.Count == 0)
        {
            return;
        }

        isLogging = true;
        ExpandLog();
        isExpandable = false;

        for (int i = 0; i < strs.Count; i++)
        {
            AddLog(strs[i]);
            await UniTask.Delay(TimeSpan.FromSeconds(f[i]), cancellationToken: _cancelTokenSource.Token);
        }

        isLogging = false;
        isExpandable = true;
        Instance.ExpandLog();
    }

    public void ExpandLog()
    {
        if(!isExpandable) return;
        
        RectTransform rectTransform = transform as RectTransform;
        
        if (!rectTransform)
        {
            return;
        }

        if (_isExpanded)
        {
            _isExpanded = false;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 585);
            transform.position += new Vector3(0, 1.6f, 0);

            if (_scrollRect)
            {
                _scrollRect.normalizedPosition = new Vector2(_scrollRect.normalizedPosition.x, 0f);
            }
        }
        else
        {
            _isExpanded = true;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 1500);
            transform.position -= new Vector3(0, 1.6f, 0);
        }
    }
}
