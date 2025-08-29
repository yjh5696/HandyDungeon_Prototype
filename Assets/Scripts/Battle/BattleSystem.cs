using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public static class BattleSystem
{
    public static void ExecuteAttack(Character attacker, Character target, CardDataSO card, int diceValue)
    {
        State cardElement = (State)System.Enum.Parse(typeof(State), card.Element);
        State deBuffType = (State)System.Enum.Parse(typeof(State), card.Debuff_Type);

        // 0. 주사위 값 수정
        diceValue += attacker.nextTurnDiceBonus;
        diceValue = diceValue * attacker.nextTurnDiceMultiplier;
        Debug.Log($"[Attack] 수정된 주사위 값: {diceValue} (보너스: {attacker.nextTurnDiceBonus}, 배수: {attacker.nextTurnDiceMultiplier})");
        attacker.ClearDiceBouns();

        int fervorDamage = 0;
        // 1. 기본 데미지 계산
        float baseDamage = card.min_Value * (diceValue * card.Calculation);

        // 2. 젖음 효과 (공격력 감소 및 회복)
        baseDamage = attacker.SetWaterEffect(baseDamage, target);

        // 3. 교란 효과 (조건부 공격력 감소)
        baseDamage = attacker.SetAirEffect(baseDamage, diceValue);

        // 3.1. 순풍 효과 (조건부 공격력 증가)
        baseDamage = attacker.SetGaleEffect(baseDamage, diceValue);

        // 4. 속성 배율
        float multiplier = ElementEffect.GetMultiplier(cardElement, target.GetCurrentElement());
        float totalDamage = baseDamage * multiplier;

        // 5. 소화 효과 (받는 피해 감소)
        totalDamage = target.SetBurndownEffect(totalDamage);
        totalDamage = Mathf.Round(totalDamage * 10f) / 10f;

        // 5.1. 균열 효과 (받는 피해 증가)
        totalDamage = target.SetLandEffect(totalDamage);
        totalDamage = Mathf.Round(totalDamage * 10f) / 10f;

        // 5.2. 수호 효과 (받는 피해 감소)
        totalDamage = target.SetGuardEffect(totalDamage);
        totalDamage = Mathf.Round(totalDamage * 10f) / 10f;

        // 5.3. 열정 효과 (데미지 추가)
        fervorDamage = attacker.SetFervorDamage();
        totalDamage += fervorDamage;

        totalDamage = Mathf.Max(0, totalDamage); // 최소 데미지 1로 설정

        // 6. HP 감소
        target.HitDamage(totalDamage);

        // 6.1. 진동 효과 (반사 피해)
        target.SetVibrationEffect(totalDamage, attacker);

        // 6.1. 진동 효과로 인한 사망 여부 체크 후 처리
        if (attacker.GetCurrentHp() <= 0)
        {
            if (target is Player player)
            {
                player.PlayerDie();
            }
            else if (attacker is Enemy enemy)
            {
                enemy.EnemyDie();
            }
            return;
        }

        // 7. 체력 감소 로그 출력
        LogManager.Instance.AddLog($"{attacker.GetUnitName()} → {target.GetUnitName()} : {totalDamage} 데미지");
        Debug.Log($"{attacker.GetUnitName()} → {target.GetUnitName()} : {totalDamage} 데미지 / {target.GetUnitName()} : {target.GetCurrentHp()}");
        
        // 8.속성 디버프 적용 
        string effectLog = ElementEffect.ApplyElementEffect(attacker, target, deBuffType, card.Debuff_Stack);

        // 9. 속성 디버프 로그 출력
        if (!string.IsNullOrEmpty(effectLog))
            LogManager.Instance.AddLog(effectLog);

        // 10. 사망 여부 체크 후 처리
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

        // 11. 소화 스택 삭제
        attacker.SetBurndownClear();
    }
    public static void ExecuteDefence(Character attacker, Character target, CardDataSO card, int diceValue)
    {
        // 0. 주사위 값 수정
        diceValue += attacker.nextTurnDiceBonus;
        diceValue = diceValue * attacker.nextTurnDiceMultiplier;

        attacker.ClearDiceBouns();

        State buffType = (State)System.Enum.Parse(typeof(State), card.Buff_Type);
        string cardType = card.Effect_Type;

        // 1. 기본 회복량 계산
        float baseDamage = card.min_Value * (diceValue * card.Calculation);
        Debug.Log($"[Support] 기본 회복량: {baseDamage}");

        // 1.1 기본 회복 및 보호막
        if(cardType == "Heal")
        {
            attacker.SetHeal(baseDamage);
        }
        else if(cardType == "Sheild")
        {
            attacker.SetShield(baseDamage);
        }


        // 2. 속성 연계 확인 및 속성 버프 부여
        string effectLog = ElementBuff.ApplyBuff(attacker, target, buffType, card.Buff_Stack);

        if (!string.IsNullOrEmpty(effectLog))
            LogManager.Instance.AddLog(effectLog);

        // 3. 재생 효과 발동
        attacker.SetRecovery(diceValue, cardType);
    }
    public static void ExecuteSpecial(Character attacker, Character target, CardDataSO card, int diceValue)
    {
        diceValue += attacker.nextTurnDiceBonus;
        diceValue = diceValue * attacker.nextTurnDiceMultiplier;
        Debug.Log($"[Attack] 수정된 주사위 값: {diceValue} (보너스: {attacker.nextTurnDiceBonus}, 배수: {attacker.nextTurnDiceMultiplier})");
        attacker.ClearDiceBouns();
        switch (card.C_Name)
        {
            case "SP1":
                // 예: 디버프 스택 모두 초기화
                attacker.ClearDebuffStacks();
                LogManager.Instance.AddLog($"{attacker.GetUnitName()}의 {card.C_Name} 효과: 디버프 스택 초기화");
                break;

            case "SP2":
                // 예: 다음 턴 주사위 ×2
                attacker.NextTurnDiceMultiplier(2);
                LogManager.Instance.AddLog($"{attacker.GetUnitName()}의 {card.C_Name} 효과: 다음 턴 주사위 ×2");
                break;

            case "SP3":
                // 예: 다음 턴 주사위 +3
                attacker.AddNextTurnDiceBouns(3);
                LogManager.Instance.AddLog($"{attacker.GetUnitName()}의 {card.C_Name} 효과: 다음 턴 주사위 +3");
                break;

            default:
                Debug.LogWarning($"알 수 없는 스페셜 카드 효과: {card.C_Name}");
                break;
        }
    }
}

