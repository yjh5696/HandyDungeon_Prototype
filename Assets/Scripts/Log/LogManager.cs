using System;
using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        text.text = "";
    }

    public void AddLog(string msg) //로그 추가하기
    {
        if(text)
            text.text += "\n" + msg;
    }

    public void AddSpacingLine()
    {
        if (text)
            text.text += "\n----------------------------------\n";
    }
}
