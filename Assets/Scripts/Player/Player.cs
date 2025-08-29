using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class Player : Character
{
    private void Awake()
    {
        GetCardsFromAsset();
    }

    private void GetCardsFromAsset()
    {
        this.cards = Resources.LoadAll<CardDataSO>("CardDataSOs").ToList();
    }

    public void PlayerDie()
    {
        Debug.Log("플레이어가 사망했습니다.");
        GameManager.Instance.EndBattle(3f).Forget();
    }
}
