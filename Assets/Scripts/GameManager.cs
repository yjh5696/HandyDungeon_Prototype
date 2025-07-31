using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isPlayerTurn = true;
    public Attack_Button_DiceRoll diceRoll;
    [SerializeField] private List<EnemySO> enemySOs;
    [SerializeField] private List<CardSO> cardSOs;
    [SerializeField] private GameObject log;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject card;
    [SerializeField] private GameObject action;
    [SerializeField] private GameObject dice;
    [SerializeField] private GameObject dicebtn;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        EnemyManager.Instance.SpawnEnemy(enemySOs[0]);
        StartPlayerTurn();
    }

    public void SwitchTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if (isPlayerTurn)
        {
            log.SetActive(true);
            buttons.SetActive(true);
            card.SetActive(false);
            action.SetActive(false);
            dice.SetActive(false);
            dicebtn.SetActive(true);
            StartPlayerTurn();
        }
        else
            StartEnemyTurn();
    }

    public void StartPlayerTurn()
    {
        LogManager.Instance.AddLog("당신의 차례입니다. 액션을 선택해주세요.");
        LogManager.Instance.AddLog("현재 나의 체력: " + PlayerManager.Instance.Player.GetCurrentHp() + " / " + PlayerManager.Instance.Player.GetMaxHp() + ".");
        LogManager.Instance.AddLog("현재 적의 체력: " + EnemyManager.Instance.Enemy.GetCurrentHp() + " / " + EnemyManager.Instance.Enemy.GetMaxHp() + ".");
        CardManager.Instance.DrawCard();
    }

    public void StartEnemyTurn()
    {
        if (!EnemyManager.Instance.Enemy)
        {
            isPlayerTurn = !isPlayerTurn;
            return;
        }
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog("적의 차례입니다.");
        EnemyManager.Instance.Enemy.DrawAndUseCard();
    }
}
