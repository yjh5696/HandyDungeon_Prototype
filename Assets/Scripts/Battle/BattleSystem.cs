using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public static class BattleSystem
{
    public static void ExecuteAttack(Character attacker, Character target, CardSO card, int diceValue)
    {
        int fervorDamage = 0;
        // 1. 기본 데미지 계산
        float baseDamage = card.Damage * (diceValue * card.DiceMultiplier);

        // 2. 침식 효과 (공격력 감소 및 회복)
        baseDamage = attacker.SetWaterEffect(baseDamage, target);

        // 3. 풍식 효과 (조건부 공격력 감소)
        baseDamage = attacker.SetWindEffect(baseDamage, diceValue);

        // 3.1. 질풍 효과 (조건부 공격력 증가)
        baseDamage = attacker.SetGaleEffect(baseDamage, diceValue);

        // 4. 속성 배율
        float multiplier = ElementEffect.GetMultiplier(card.State, target.GetCurrentElement());
        float totalDamage = baseDamage * multiplier;

        // 5. 소화 효과 (받는 피해 감소)
        totalDamage = target.SetBurndownEffect(totalDamage);
        totalDamage = Mathf.Round(totalDamage * 10f) / 10f;

        // 5.1. 진창 효과 (받는 피해 증가)
        totalDamage = target.SetEarthEffect(totalDamage);
        totalDamage = Mathf.Round(totalDamage * 10f) / 10f;

        // 5.2. 수호 효과 (받는 피해 감소)
        totalDamage = target.SetGuardEffect(totalDamage);
        totalDamage = Mathf.Round(totalDamage * 10f) / 10f;

        // 5.3. 열정 효과 (데미지 추가)
        fervorDamage = attacker.SetFervorDamage();
        totalDamage += fervorDamage;

        // 6. HP 감소
        target.HitDamage(totalDamage);

        // 6.1. 진동 효과 (반사 피해)
        target.SetVibrationEffect(totalDamage, attacker);

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
        string effectLog = ElementEffect.ApplyElementEffect(attacker, target, card.State, 1);
        

        // 10. 속성 디버프 로그 출력
        if (!string.IsNullOrEmpty(effectLog))
            LogManager.Instance.AddLog(effectLog);

        // 11. 소화 스택 삭제
        attacker.SetBurndownClear();
    }
    public static void ExecuteDefence(Character attacker, Character target, CardSO card, int diceValue)
    {
        // 1. 기본 데미지 계산
        float baseDamage = card.Damage * card.DiceMultiplier;

        // 2. 속성 연계 확인 및 속성 버프 부여
        string effectLog = ElementBuff.ApplyBuff(attacker, target, card.State, 3);

        if (!string.IsNullOrEmpty(effectLog))
            LogManager.Instance.AddLog(effectLog);

        // 3. 재생 효과 발동
        attacker.SetRecovery(diceValue, (int)baseDamage);
    }
}

