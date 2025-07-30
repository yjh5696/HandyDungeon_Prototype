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
        Debug.Log($"Player���� {status} �����̻� ����!");
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
        Debug.Log($"Player��(��) {modifiedDamage}�� ���ظ� �޾ҽ��ϴ�! ���� ü��: {GetCurrentHp()}");
        if (GetCurrentHp() <= 0)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        Debug.Log($"Player��(��) ����߽��ϴ�!");

        // ��� �� EnemyManager�� �˸� (�ð� �� ������Ʈ ó�� ���)
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerDied();
        }
    }
}
