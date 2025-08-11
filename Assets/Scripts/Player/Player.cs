using System;
using UnityEngine;
using System.Collections;

public class Player : Character
{
    public void PlayerDie()
    {
        Debug.Log("플레이어가 사망했습니다.");
        GameManager.Instance.EndGame();
    }
}
