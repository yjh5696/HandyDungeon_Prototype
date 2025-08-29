using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private EnemySO _enemySo;
    private CardDataSO _currentEnemyCard;
    public CardDataSO CurrentEnemyCard => _currentEnemyCard;

    public void SetEnemySo(EnemySO enemy)
    {
        _enemySo = enemy;

        // EnemySO 내 카드 덱을 복사하여 할당
        cards = new List<CardDataSO>(enemy.EnemyCards);
        Debug.Log($"EnemySO '{enemy.EnemyName}' 카드 수: {enemy.EnemyCards?.Count ?? -1}, Enemy 객체 cards 수: {cards?.Count ?? -1}");
    }

    public EnemySO GetEnemySo()
    {
        return _enemySo;
    }

    // 외부에서 카드덱 직접 할당 가능
    public void SetCards(List<CardDataSO> cardPool)
    {
        cards = cardPool;
    }


    public override void DrawAndUseCard()
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("적 카드 덱이 비어있거나 설정되지 않았습니다.");
            return;
        }

        int result = Random.Range(0, cards.Count);
        _currentEnemyCard = cards[result];

        CardManager.Instance.selectedCard = _currentEnemyCard;

        LogManager.Instance.AddLog($"{_enemySo.EnemyName}이/가 {_currentEnemyCard.C_Name}을 사용하였습니다!");
        GameManager.Instance.diceRoll.OnAttackButtonClicked();
    }

    public void EnemyDie()
    {
        Debug.Log($"{_enemySo.EnemyName}이(가) 사망했습니다!");
        if (EnemyManager.Instance)
        {
            EnemyManager.Instance.OnEnemyDied();
            GameManager.Instance.EndBattle(3f).Forget();
        }
        EnemyManager.Instance.StartCoroutine(DestroyEnemyAfterDelay(3f));
    }

    private IEnumerator DestroyEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (this != null)
        {
            Destroy(gameObject);
        }
    }
}
