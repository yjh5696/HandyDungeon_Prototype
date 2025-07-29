using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    private EnemySO _enemySo;
    private CardSO _currentEnemyCard;

    private StatusEffect currentStatus = StatusEffect.None;
    private int statusTurnCount = 0;

    public enum StatusEffect
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Wind = 3,
    }
    public EnemySO GetEnemySo()
    {
        return _enemySo;
    }
    public void SetEnemySo(EnemySO enemy)
    {
        _enemySo = enemy;
    }

    public void ApplyStatus(StatusEffect status)
    {
        currentStatus = status;
        statusTurnCount = 1;  // 1턴 유지
        Debug.Log($"{_enemySo.Name}에게 {status} 상태이상 적용!");
    }

    public void OnTurnEnd()
    {
        if (statusTurnCount > 0)
        {
            statusTurnCount--;
            if (statusTurnCount <= 0)
            {
                Debug.Log($"{_enemySo.Name}의 상태이상 {currentStatus}가 사라졌습니다.");
                currentStatus = StatusEffect.None;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        float modifiedDamage = damage;

        switch (currentStatus)
        {
            case StatusEffect.Fire:
                modifiedDamage += 5f;
                break;
            case StatusEffect.Water:
                modifiedDamage -= 5f;
                break;
            case StatusEffect.Wind:
                modifiedDamage *= 0.9f;
                break;
        }

        SetCurrentHp(GetCurrentHp() - modifiedDamage);
        Debug.Log($"{_enemySo.Name}이(가) {modifiedDamage}의 피해를 받았습니다! 현재 체력: {GetCurrentHp()}"); 
        if (GetCurrentHp() <= 0)
        {
            Die();
        }
    }

    // 적 사망 처리 함수
    private void Die()
    {
        Debug.Log($"{_enemySo.Name}이(가) 사망했습니다!");

        // 사망 시 EnemyManager에 알림 (시각 및 오브젝트 처리 담당)
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyDied();
        }
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
