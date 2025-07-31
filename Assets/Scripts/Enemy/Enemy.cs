using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    private EnemySO _enemySo;
    private CardSO _currentEnemyCard;

    private EnemyStatusEffect currentStatus = EnemyStatusEffect.None;
    private int statusTurnCount = 0;

    public enum EnemyStatusEffect
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

    public void EnemyApplyStatus(EnemyStatusEffect status)
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
                currentStatus = EnemyStatusEffect.None;
            }
        }
    }

    public void EnemyTakeDamage(float damage, EnemyStatusEffect status)
    {
        float modifiedDamage = damage;

        switch (currentStatus)
        {
            case EnemyStatusEffect.Fire:
                if(status == EnemyStatusEffect.Water)
                {
                    modifiedDamage *= 0.8f;
                }
                else if (status == EnemyStatusEffect.Wind)
                {
                    modifiedDamage *= 1.2f;
                }
                else if (status == EnemyStatusEffect.Fire)
                {
                    modifiedDamage *= 1f;
                }
                else
                {
                    modifiedDamage *= 1.0f;
                }
                break;
            case EnemyStatusEffect.Water:
                if (status == EnemyStatusEffect.Water)
                {
                    modifiedDamage *= 1f;
                }
                else if (status == EnemyStatusEffect.Wind)
                {
                    modifiedDamage *= 1.2f;
                }
                else if (status == EnemyStatusEffect.Fire)
                {
                    modifiedDamage *= 0.8f;
                }
                else
                {
                    modifiedDamage *= 1.0f;
                }
                break;
            case EnemyStatusEffect.Wind:
                if (status == EnemyStatusEffect.Water)
                {
                    modifiedDamage *= 0.8f;
                }
                else if (status == EnemyStatusEffect.Wind)
                {
                    modifiedDamage *= 1f;
                }
                else if (status == EnemyStatusEffect.Fire)
                {
                    modifiedDamage *= 1.2f;
                }
                else
                {
                    modifiedDamage *= 1.0f; // 불 상태로 인해 추가 피해
                }
                break;
        }

        SetCurrentHp(GetCurrentHp() - modifiedDamage);
        LogManager.Instance.AddLog($"{EnemyManager.Instance.Enemy.GetEnemySo().Name}에게 {modifiedDamage}의 데미지를 주었습니다!");
        
        if (GetCurrentHp() <= 0)
        {
            EnemyDie();
        }
    }

    // 적 사망 처리 함수
    public void EnemyDie()
    {
        Debug.Log($"{_enemySo.Name}이(가) 사망했습니다!");

        // 사망 시 EnemyManager에 알림 (시각 및 오브젝트 처리 담당)
        if (EnemyManager.Instance != null)
        {
            GameManager.Instance.EndGame();
            //EnemyManager.Instance.OnEnemyDied();
            //GameManager.Instance.SwitchTurn();
        }
    }

    public void DrawAndUseCard()
    {
        List<CardSO> cards = _enemySo.EnemyCards;
        int result = Random.Range(0, cards.Count);
        _currentEnemyCard = cards[result];

        CardManager.Instance.selectedCard = _currentEnemyCard;

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
