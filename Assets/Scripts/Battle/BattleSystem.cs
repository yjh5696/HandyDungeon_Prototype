using Unity.VisualScripting;
using UnityEngine;
using System.Collections;


public static class BattleSystem
{
    public static void ExecuteAttack(Character attacker, Character target, CardSO card, int diceValue)
    {
        // 1. 침식 보정 (주사위 눈 감소)
        diceValue = attacker.SetWaterEffect(diceValue);

        // 2. 기본 데미지 계산
        float baseDamage = card.Damage * (diceValue * card.DiceMultiplier);

        // 3. 풍식 효과 (공격력 감소)
        baseDamage = attacker.SetWindEffect(baseDamage, diceValue);

        // 4. 속성 배율
        float multiplier = ElementEffect.GetMultiplier(card.State, target.GetCurrentElement());
        float totalDamage = baseDamage * multiplier;

        // 5. 진창 효과 (받는 피해 증가)
        totalDamage = target.SetEarthEffect(totalDamage);
        totalDamage = Mathf.Round(totalDamage * 10f) / 10f;

        // 6. HP 감소
        float clearHp = target.GetCurrentHp() - totalDamage;
        clearHp = Mathf.Round(clearHp * 10f) / 10f;
        target.SetCurrentHp(clearHp);

        // 7. 체력 감소 로그 출력
        LogManager.Instance.AddLog($"{attacker.GetUnitName()} → {target.GetUnitName()} : {totalDamage} 데미지");

        // 8. 사망 여부 체크 후 처리
        if (target.GetCurrentHp() <= 0)
        {
            if (target is Player player)
            {
                player.PlayerDie();
            }
            else if (target is Enemy enemy)
            {
                enemy.EnemyDie();
            }
            return;
        }

        // 9. 속성 디버프 적용
        string effectLog = ElementEffect.ApplyElementEffect(target, card.State, 1);
        

        // 10. 속성 디버프 로그 출력
        if (!string.IsNullOrEmpty(effectLog))
            LogManager.Instance.AddLog(effectLog);
    }
}

