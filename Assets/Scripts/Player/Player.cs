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

    public void PlayerDie()
    {
        Debug.Log("플레이어가 사망했습니다.");
        //GameManager.Instance.EndBattle(3f).Forget();
    }
}
