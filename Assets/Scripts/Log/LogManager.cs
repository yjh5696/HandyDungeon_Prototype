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
        
        AddLog("당신의 차례입니다. 액션을 선택해주세요.");
        AddLog("현재 나의 체력: " + PlayerManager.Instance.GetCurrentHp() + " / " + PlayerManager.Instance.GetMaxHp() + ".");
        //AddLog("현재 적의 체력: " + EnemyManager.Instance._enemy.GetCurrentHp() + "/" + EnemyManager.Instance._enemy.GetMaxHp() + ".");
    }

    public void AddLog(string msg) //로그 추가하기
    {
        if(text != null)
            text.text += "\n" + msg;
    }
}
