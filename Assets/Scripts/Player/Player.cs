using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Player : Character
{
    [SerializeField] private List<CardDataSO> initCards = new List<CardDataSO>();

    private void Awake()
    {
        InitPLayerCards();
    }

    private void InitPLayerCards()
    {
        Cards = initCards;
    }
    
    public CardDataSO GetCard(int cardID)
    {
        CardDataSO[] tmpCards = Resources.LoadAll<CardDataSO>("CardDataSOs").ToArray();
        if (cardID == 0)
        {
            int r = Random.Range(0, tmpCards.Length);
            Cards.Add(tmpCards[r]);
            return tmpCards[r];
        }
        else
        {
            foreach (CardDataSO card in tmpCards)
            {
                if (card.C_Id != cardID)
                {
                    continue;
                }

                Cards.Add(card);
                return card;
            }
        }
        return null;
    }

    public void PlayerDie()
    {
        Debug.Log("플레이어가 사망했습니다.");
        GameManager.Instance.EndBattle(3f).Forget();
    }
}
