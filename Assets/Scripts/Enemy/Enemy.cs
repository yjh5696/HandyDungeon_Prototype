using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    private EnemySO _enemySo;
    private CardSO _currentEnemyCard;

    public EnemySO GetEnemySo()
    {
        return _enemySo;
    }
    public void SetEnemySo(EnemySO enemy)
    {
        _enemySo = enemy;
    }
    
    public void DrawAndUseCard()
    {
        List<CardSO> cards = _enemySo.EnemyCards;
        int result = Random.Range(0, cards.Count);
        _currentEnemyCard = cards[result];
        
        switch (_currentEnemyCard.Style)
        {
            case Style.Attack:
                break;
            case Style.Defence:
                break;
            case Style.Special:
                break;
            default:
                break;
        }
        
        LogManager.Instance.AddLog($"{_enemySo.Name}이/가 {_currentEnemyCard.CardName}을 사용하였습니다!");
        GameManager.Instance.diceRoll.OnAttackButtonClicked();
        
    }
}
