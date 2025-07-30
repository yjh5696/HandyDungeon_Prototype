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
        //statusTurnCount = 1;  // 1턴 유지
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
                    modifiedDamage *= 1.0f; // 불 상태로 인해 추가 피해
                }
                break;
        }

        SetCurrentHp(GetCurrentHp() - modifiedDamage);
        Debug.Log($"Player이(가) {modifiedDamage}의 피해를 받았습니다! 현재 체력: {GetCurrentHp()}");
        if (GetCurrentHp() <= 0)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        Debug.Log($"Player이(가) 사망했습니다!");

        // 사망 시 EnemyManager에 알림 (시각 및 오브젝트 처리 담당)
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerDied();
        }
    }
}
