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

    public void Log(string msg)
    {
        if(text != null)
            text.text += "\n" + msg;
    }
}
