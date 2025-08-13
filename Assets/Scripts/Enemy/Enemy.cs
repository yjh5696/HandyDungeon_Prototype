using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    private EnemySO _enemySo;
    private CardSO _currentEnemyCard;

    public EnemySO GetEnemySo() => _enemySo;
    public void SetEnemySo(EnemySO enemy) => _enemySo = enemy;

    // 사망 처리
    public void EnemyDie()
    {
        Debug.Log($"{_enemySo.Name}이(가) 사망했습니다!");
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyDied();
            GameManager.Instance.EndGame();
        }
    }

    // 카드 뽑고 사용
    public void DrawAndUseCard()
    {
        List<CardSO> cards = _enemySo.EnemyCards;
        int result = Random.Range(0, cards.Count);
        _currentEnemyCard = cards[result];

        CardManager.Instance.selectedCard = _currentEnemyCard;

        LogManager.Instance.AddLog($"{_enemySo.Name}이/가 {_currentEnemyCard.CardName}을 사용하였습니다!");
        GameManager.Instance.diceRoll.OnAttackButtonClicked();
    }
}
