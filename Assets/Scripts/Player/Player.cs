using System;
using UnityEngine;
using static Enemy;

public class Player : Character
{
    private PlayerStatusEffect currentStatus = PlayerStatusEffect.None;
    //private int statusTurnCount = 0;

    public enum PlayerStatusEffect
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Wind = 3,
    }

    public void PlayerApplyStatus(PlayerStatusEffect status)
    {
        currentStatus = status;
        //statusTurnCount = 1;  // 1�� ����
        Debug.Log($"Player에게 {status} 상태이상 적용!");
    }

    public void PlayerTakeDamage(float damage, PlayerStatusEffect status)
    {
        float modifiedDamage = damage;

        switch (currentStatus)
        {
            case PlayerStatusEffect.Fire:
                if (status == PlayerStatusEffect.Water)
                {
                    modifiedDamage *= 0.8f;
                }
                else if (status == PlayerStatusEffect.Wind)
                {
                    modifiedDamage *= 1.2f;
                }
                else if (status == PlayerStatusEffect.Fire)
                {
                    modifiedDamage *= 1f;
                }
                else
                {
                    modifiedDamage *= 1.0f;
                }
                break;
            case PlayerStatusEffect.Water:
                if (status == PlayerStatusEffect.Water)
                {
                    modifiedDamage *= 1f;
                }
                else if (status == PlayerStatusEffect.Wind)
                {
                    modifiedDamage *= 1.2f;
                }
                else if (status == PlayerStatusEffect.Fire)
                {
                    modifiedDamage *= 0.8f;
                }
                else
                {
                    modifiedDamage *= 1.0f;
                }
                break;
            case PlayerStatusEffect.Wind:
                if (status == PlayerStatusEffect.Water)
                {
                    modifiedDamage *= 0.8f;
                }
                else if (status == PlayerStatusEffect.Wind)
                {
                    modifiedDamage *= 1f;
                }
                else if (status == PlayerStatusEffect.Fire)
                {
                    modifiedDamage *= 1.2f;
                }
                else
                {
                    modifiedDamage *= 1.0f; // �� ���·� ���� �߰� ����
                }
                break;
        }

        SetCurrentHp(GetCurrentHp() - modifiedDamage);
        LogManager.Instance.AddLog($"플레이어에게 {modifiedDamage}의 데미지를 주었습니다!");
        if (GetCurrentHp() <= 0)
        {
            PlayerDie();
        }
    }

    public void PlayerDie()
    {
        Debug.Log($"Player��(��) ����߽��ϴ�!");

        // ��� �� EnemyManager�� �˸� (�ð� �� ������Ʈ ó�� ���)
        if (PlayerManager.Instance != null)
        {
            //PlayerManager.Instance.OnPlayerDied();
            GameManager.Instance.EndGame();
        }
    }
}
