using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.Rendering.GPUSort;

public class Enemy : Character
{
    private EnemySO _enemySo;
    private CardDataSO _currentEnemyCard;
    public CardDataSO CurrentEnemyCard => _currentEnemyCard;

    private bool hasUsedSupportCard = false;

    public SpriteRenderer spriteRenderer;
    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void SetEnemySo(EnemySO enemy)
    {
        _enemySo = enemy;

        spriteRenderer.sprite = enemy.Sprite;
        spriteRenderer.flipX = enemy.flipX;

        // EnemySO 내 카드 덱을 복사하여 할당
        Cards = new List<CardDataSO>(enemy.EnemyCards);
        Debug.Log($"EnemySO '{enemy.EnemyName}' 카드 수: {enemy.EnemyCards?.Count ?? -1}, Enemy 객체 cards 수: {Cards?.Count ?? -1}");
    }

    public EnemySO GetEnemySo()
    {
        return _enemySo;
    }

    // 외부에서 카드덱 직접 할당 가능
    public void SetCards(List<CardDataSO> cardPool)
    {
        Cards = cardPool;
    }


    public override void DrawAndUseCard()
    {
        if (Cards == null || Cards.Count == 0)
        {
            Debug.LogWarning("적 카드 덱이 비어있거나 설정되지 않았습니다.");
            return;
        }

        float healthRatio = (float)EnemyManager.Instance.Enemy.GetCurrentHp() / EnemyManager.Instance.Enemy.GetMaxHp();
        List<CardDataSO> filteredCards;

        if (healthRatio <= 0.5f && !hasUsedSupportCard)
        {
            filteredCards = Cards.Where(card => card.C_Type == "Support").ToList();

            if (filteredCards.Count > 0)
            {
                int result = Random.Range(0, filteredCards.Count);
                _currentEnemyCard = filteredCards[result];
                hasUsedSupportCard = true; // 1번 사용 후 true 처리
            }
            else
            {
                Debug.LogWarning("지원 카드가 없습니다.");
                return;
            }
        }
        else
        {
            filteredCards = Cards.Where(card => card.C_Type == "Action").ToList();
            if (filteredCards.Count == 0)
                filteredCards = Cards;

            int result = Random.Range(0, filteredCards.Count);
            _currentEnemyCard = filteredCards[result];
        }

        CardManager.Instance.selectedCard = _currentEnemyCard;
        LogManager.Instance.AddLog($"{_enemySo.EnemyName}이/가 {_currentEnemyCard.C_Name}을 사용하였습니다!");
        GameManager.Instance.diceRoll.TryEnemyProCessDiceResult(_currentEnemyCard);
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
