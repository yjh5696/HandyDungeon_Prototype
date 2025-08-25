using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;
    public bool isLogging = false;
    private CancellationTokenSource _cancelTokenSource;
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _cancelTokenSource = new CancellationTokenSource();
        
        text.text = "";
    }

    public void Clear()
    {
        text.text = "";
    }

    public void CancelLog()
    {
        _cancelTokenSource.Cancel();
        _cancelTokenSource.Dispose();
        
        isLogging = false;
        _cancelTokenSource = new CancellationTokenSource();
    }

    public void AddLog(string msg) //로그 추가하기
    {
        if(text)
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
        if (text)
            text.text += "\n----------------------------------\n";
    }

    public async UniTaskVoid StartLog(string str)
    {
        await UniTask.WaitUntil(() => !isLogging);
        
        isLogging = true;
        string[] lines = str.Split('{');
        foreach (string line in lines)
        {
            if (_cancelTokenSource.Token.IsCancellationRequested)
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

            await UniTask.Delay(TimeSpan.FromSeconds(double.TryParse(wt, out double t) ? t : 0), cancellationToken: _cancelTokenSource.Token);
            AddLog(log);
        }
        
        AddSpacingLine();
        isLogging = false;
    }
}
