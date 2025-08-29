using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class Character : MonoBehaviour
{
    public string unitName;
    protected float MaxHp;
    protected float CurrentHp;
    protected State CurrentState;
    protected HPBar HpBar;
    public event EventHandler OnHpChanged;
    public int nextTurnDiceBonus = 0;
    public int nextTurnDiceMultiplier = 1;

    protected State CurrentElement = State.None;
    protected Dictionary<State, int> elementStacks = new Dictionary<State, int>();
    protected int extraDamageStacks = 0;
    protected int fervorDamage = 0;
    protected float RecoveryValue = 0;
    protected float Recovery = 0;
    protected float ShieldValue = 0;
    protected float Shield = 0;

    [SerializeField] int maxElementStack = 10;

    // 최근 맞은 속성 기록 (유지턴 제거)
    protected State lastElement = State.None;
    protected State lastBuffElement = State.None;

    public State GetLastElement() => lastElement;
    public State GetLastBuffElement() => lastBuffElement;
    public State GetCurrentElement() => CurrentElement;

    public void SetLastElement(State element)
    {
        Debug.Log($"[SetLastElement] {unitName} : {lastElement} → {element}");
        lastElement = element;
    }

    public void SetLastBuffElement(State element)
    {
        Debug.Log($"[SetLastBuffElement] {unitName} : {lastBuffElement} → {element}");
        lastBuffElement = element;
    }

    [SerializeField] public List<CardDataSO> cards; // 인스펙터에서 직접 카드 풀 할당 가능
    public List<CardDataSO> Cards => cards; // 외부에서 읽기 전용으로 노출
    protected CardDataSO currentCard;

    public virtual void DrawAndUseCard()
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning($"{name} 카드 덱이 없습니다.");
            return;
        }
        int result = Random.Range(0, cards.Count);
        currentCard = cards[result];
        CardManager.Instance.selectedCard = currentCard;
        LogManager.Instance.AddLog($"{name}이/가 {currentCard.C_Name}을 사용했습니다.");
    }
    private void Start() => OnHpChanged += HpChanged;

    public void SetUnitName(string name) => unitName = name;
    public string GetUnitName() => unitName;
    public float GetMaxHp() => MaxHp;
    public float GetCurrentHp() => CurrentHp;
    public State GetCurrentState() => CurrentState;
    public void SetCurrentState(State state) => CurrentState = state;
    public void SetMaxHp(float maxHp) => MaxHp = maxHp;
    public void SetHpBar(HPBar hpBar) => HpBar = hpBar;

    public void SetCurrentHp(float currentHp)
    {
        CurrentHp = Mathf.Clamp(currentHp, 0, MaxHp);
        OnHpChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void HpChanged(object sender, EventArgs e) =>
        HpBar.HpChanged(CurrentHp, MaxHp);

    public void AddStatusStacks(State element, int stacks)
    {
        if (elementStacks.ContainsKey(element))
        {
            elementStacks[element] = Mathf.Min(elementStacks[element] + stacks, maxElementStack);
        }
        else
        {
            elementStacks[element] = Mathf.Min(stacks, maxElementStack);
        }

        CurrentElement = element;

        if (element == State.Fire || element == State.Water || element == State.Air || element == State.Land)
            SetLastElement(element); // 즉시 반영
        else if (element == State.Fervor || element == State.Gale || element == State.Guard || element == State.Recovery)
            SetLastBuffElement(element); // 즉시 반영
    }

    public void SetStatus(string statusName, int stacks)
    {
        var mapped = MapStatusNameToState(statusName);
        elementStacks[mapped] = Mathf.Min(stacks, maxElementStack);  // 최대 10으로 제한
        elementStacks[mapped] = stacks;
        CurrentElement = mapped;
        SetLastElement(mapped); // 즉시 반영
        Debug.Log($"{unitName}에게 {statusName} 상태 {stacks} 스택 부여");
    }

    public int GetStatusStacks(State element) =>
        elementStacks.ContainsKey(element) ? elementStacks[element] : 0;

    public void RemoveStatus(State element)
    {
        if (elementStacks.ContainsKey(element))
            elementStacks.Remove(element);

        if (CurrentElement == element)
        {
            CurrentElement = State.None;
            Debug.Log($"[RemoveStatus] {unitName} : CurrentElement {element} 제거됨 (lastElement={lastElement} 유지)");
        }
    }

    State MapStatusNameToState(string statusName)
    {
        return statusName switch
        {
            "점화" => State.Fire,
            "풍식" => State.Air,
            "진창" => State.Land,
            "침식" => State.Water,
            "발화" => State.Ignition,
            "열정" => State.Fervor,
            "질풍" => State.Gale,
            "수호" => State.Guard,
            "재생" => State.Recovery,
            //"보호막" => State.Shield,
            //"회복" => State.Healing,
            "진동" => State.Vibration,
            _ => State.None
        };
    }

    // 교란: 홀짝 조건부 공격력 감소
    public float SetAirEffect(float baseDamage, int diceValue)
    {
        if (elementStacks.ContainsKey(State.Air))
        {
            int stacks = elementStacks[State.Air];
            if ((stacks % 2) == (diceValue % 2))
            {
                float original = baseDamage;
                baseDamage = Mathf.Max(0, baseDamage - stacks * 2f);
                LogManager.Instance.AddLog($"교란 효과로 공격력 {original} → {baseDamage} 감소");
            }
        }
        return baseDamage;
    }

    // 진동: 공격 시 피해 반사
    public void SetVibrationEffect(float totalDamage, Character attacker)
    {
        if (elementStacks.ContainsKey(State.Vibration))
        {
            attacker.HitDamage(totalDamage);
            LogManager.Instance.AddLog($"{unitName}의 진동 효과로 {attacker.GetUnitName()}에게 {totalDamage} 피해");
            if (attacker is Player)
            {
                PlayerManager.Instance.PlayHitAnimation();
            }
            else if (attacker is Enemy)
            {
                EnemyManager.Instance.EnemyHitAnimation();
            }
            elementStacks[State.Vibration] = Mathf.Max(0, elementStacks.GetValueOrDefault(State.Vibration, 0) - 1);
        }
    }
    // 소화 : 공격 시 피해 감소
    public float SetBurndownEffect(float totalDamage)
    {
        if (elementStacks.ContainsKey(State.Burndown))
        {
            totalDamage /= 2; // 버닝 효과로 피해 절반 감소
            elementStacks[State.Burndown] = Mathf.Max(0, elementStacks.GetValueOrDefault(State.Burndown, 0) - 1);
            LogManager.Instance.AddLog($"{unitName}의 소화 효과로 피해 {totalDamage} 감소");
        }
        return totalDamage;
    }

    // 소화 상태 제거
    public void SetBurndownClear()
    {
        if (elementStacks.ContainsKey(State.Burndown))
        {
            LogManager.Instance.AddLog($"{unitName}의 소화 상태가 사라짐");
            RemoveStatus(State.Burndown);
        }
    }

    // 순풍: 홀짝 조건부 공격력 증가
    public float SetGaleEffect(float baseDamage, int diceValue)
    {
        if(elementStacks.ContainsKey(State.Gale))
        {
            int stacks = elementStacks[State.Gale];
            if ((stacks % 2) == (diceValue % 2))
            {
                float original = baseDamage;
                baseDamage += stacks * 2f; // 순풍 효과로 공격력 증가
                LogManager.Instance.AddLog($"순풍 효과로 공격력 {original} → {baseDamage} 증가");
                elementStacks[State.Gale] = Mathf.Max(0, stacks - 1); // 스택 감소
            }
        }
        return baseDamage;
    }


    // 균열: 발동 시 스택 감소
    public float SetLandEffect(float damage)
    {
        if (elementStacks.ContainsKey(State.Land))
        {
            int stacks = elementStacks[State.Land];
            float original = damage;
            damage *= 1f + stacks * 0.1f;

            if (Mathf.Abs(original - damage) > 0.001f)
            {
                LogManager.Instance.AddLog($"균열 효과로 피해 {original} → {damage} 증가");
                stacks--;
                if (stacks <= 0)
                {
                    elementStacks.Remove(State.Land);
                    LogManager.Instance.AddLog($"{unitName}의 균열 상태가 사라짐");
                    if (CurrentElement == State.Land) CurrentElement = State.None;
                }
                else elementStacks[State.Land] = stacks;
            }
        }
        return damage;
    }

    // 수호: 발동 시 피해 감소
    public float SetGuardEffect(float Damage)
    {
        if(elementStacks.ContainsKey(State.Guard))
        {
            int stacks = elementStacks[State.Guard];
            float original = Damage;
            Damage *= 1f - stacks * 0.1f;
            if (Mathf.Abs(original - Damage) > 0.001f)
            {
                LogManager.Instance.AddLog($"수호 효과로 피해 {original} → {Damage} 감소");
                stacks--;
                if (stacks <= 0)
                {
                    elementStacks.Remove(State.Guard);
                    LogManager.Instance.AddLog($"{unitName}의 수호 상태가 사라짐");
                    if (CurrentElement == State.Guard) CurrentElement = State.None;
                }
                else elementStacks[State.Guard] = stacks;
            }
        }
        return Damage;
    }

    // 젖음: 발동 시 스택 감소
    public float SetWaterEffect(float baseDamage, Character target)
    {
        if (elementStacks.ContainsKey(State.Water))
        {
            int stack = elementStacks[State.Water];

            float newHp = Mathf.Min(target.GetMaxHp(), target.GetCurrentHp() + stack);
            target.SetCurrentHp(newHp);
            LogManager.Instance.AddLog($"{unitName}이/가 젖음 효과로 {stack} 회복");

            baseDamage = Mathf.Max(0, baseDamage - stack);
            LogManager.Instance.AddLog($"{unitName}의 젖음 효과로 {stack}만큼 데미지 감소");

            elementStacks[State.Water] = Mathf.Max(0, stack - 1);
        }
        return baseDamage;
    }

    // 데미지 처리
    public void TakeDamage(float damage)
    {
        if (extraDamageStacks > 0) damage += extraDamageStacks;
        if (Shield > 0)
        {
            if (damage <= Shield)
            {
                Shield -= damage;
                LogManager.Instance.AddLog($"{unitName}이/가 보호막으로 {damage} 피해 방어 (남은 보호막: {Shield})");
                damage = 0f;
            }
        }
        SetCurrentHp(CurrentHp - damage);
    }
    
    // 데미지 처리
    public void HitDamage(float damage)
    {
        if (Shield > 0)
        {
            if (damage <= Shield)
            {
                Shield -= damage;
                LogManager.Instance.AddLog($"{unitName}이/가 보호막으로 {damage} 피해 방어 (남은 보호막: {Shield})");
                damage = 0f;
            }
        }
        float clearHp = GetCurrentHp() - damage;
        clearHp = Mathf.Round(clearHp * 10f) / 10f;
        SetCurrentHp(clearHp);
    }
    protected virtual void OnDeath() { }
    public void SetExtraDamageTaken(int stacks) => extraDamageStacks = stacks;

    // 열정 효과
    public int SetFervorDamage()
    {
        if(elementStacks.ContainsKey(State.Fervor))
        {
            int stacks = GetStatusStacks(State.Fervor);
            fervorDamage += stacks;
            LogManager.Instance.AddLog($"{unitName}의 열정 효과로 {stacks} 데미지 추가");
            elementStacks[State.Fervor] = Mathf.Max(0, elementStacks.GetValueOrDefault(State.Fervor, 0) - 1);
            
        }
        return fervorDamage;
    }

    // 재생 효과
    public void SetRecovery(int diceValue, string cardType)
    {
        if(elementStacks.ContainsKey(State.Recovery))
        {
            int stacks = elementStacks[State.Recovery];
            if(cardType == "Heal")
            {
                SetHealEffect(stacks, diceValue); // 회복 설정
            }
            else if(cardType == "Sheild")
            {
                SetShieldEffect(stacks, diceValue); // 보호막 설정
            }
            elementStacks[State.Recovery] = Mathf.Max(0, elementStacks.GetValueOrDefault(State.Recovery, 0) - 1);
        }
    }

    // 보호막 효과
    public void SetShieldEffect(int stacks, int diceValue)
    {
        if(diceValue <= 0)
        {
            diceValue = 1;
        }
        ShieldValue = stacks;
        Shield += ShieldValue * diceValue;
        LogManager.Instance.AddLog($"{unitName}이/가 보호막 효과로{Shield} 보호막 획득 (총 보호막: {Shield})");
    }
    
    // 회복 효과
    public void SetHealEffect(int stacks, int diceValue)
    {
        if (diceValue <= 0)
        {
            diceValue = 1;
        }
        RecoveryValue = stacks;
        Recovery = RecoveryValue * diceValue;
        SetCurrentHp(CurrentHp + Recovery);
        LogManager.Instance.AddLog($"{unitName}이/가 재생 효과로 {Recovery} 회복");
    }

    public void SetShield(float damage)
    {
        Shield += damage;
        LogManager.Instance.AddLog($"{unitName}이/가 {damage} 보호막 획득 (총 보호막: {Shield})");
    }

    public void SetHeal(float damage)
    {
        SetCurrentHp(CurrentHp + damage);
        LogManager.Instance.AddLog($"{unitName}이/가 {damage} 회복 (현재 체력: {CurrentHp}/{MaxHp})");
    }

    //교란 감소 (본인 턴 종료 시)
    public virtual void OnTurnEnd_WindDecrease()
    {
        if (elementStacks.ContainsKey(State.Air))
        {
            int stacks = elementStacks[State.Air] - 1;
            if (stacks <= 0)
            {
                elementStacks.Remove(State.Air);
                LogManager.Instance.AddLog($"{unitName}의 교란 상태가 사라짐");
                if (CurrentElement == State.Air) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Air] = stacks;
                LogManager.Instance.AddLog($"{unitName}의 교란 스택 1 감소 (남은 스택: {stacks})");
            }
        }
    }

    // 디버프 스택 초기화
    public void ClearDebuffStacks()
    {
        // 디버프 상태들(State.Fire, Water, Air, Land)만 따로 관리
        State[] debuffs = new State[] { State.Fire, State.Water, State.Air, State.Land, State.Ignition };
        foreach (var debuff in debuffs)
        {
            if (elementStacks.ContainsKey(debuff))
                elementStacks.Remove(debuff);
        }

        // CurrentElement가 디버프 상태면 초기화
        if (Array.Exists(debuffs, d => d == CurrentElement))
            CurrentElement = State.None;

        LogManager.Instance.AddLog($"{unitName}의 디버프 스택이 초기화되었습니다.");
    }

    // 다음 턴 주사위 보너스
    public void AddNextTurnDiceBouns(int bonus)
    {
        nextTurnDiceBonus += bonus;
        LogManager.Instance.AddLog($"{unitName}의 다음 턴 주사위 값이 {bonus}만큼 증가합니다.");
    }

    // 다음 턴 주사위 2배
    public void NextTurnDiceMultiplier(int multiplier)
    {
        nextTurnDiceMultiplier *= multiplier;
        LogManager.Instance.AddLog($"{unitName}의 다음 턴 주사위 값이 {multiplier}배로 증가합니다.");
    }

    public void ClearDiceBouns()
    {
        nextTurnDiceBonus = 0;
        nextTurnDiceMultiplier = 1;
    }

    // DOT 처리 (점화, 연소)
    public virtual void ProcessEndTurnEffects()
    {
        List<State> removeList = new List<State>();

        foreach (var kvp in new Dictionary<State, int>(elementStacks))
        {
            State element = kvp.Key;
            int stacks = kvp.Value;
            bool decrease = false;

            if (element == State.Fire)
            {
                TakeDamage(stacks);
                LogManager.Instance.AddLog($"점화 효과로 {unitName}에게 {stacks} 피해");
                decrease = true;
            }
            else if (element == State.Ignition)
            {
                TakeDamage(stacks * 3);
                LogManager.Instance.AddLog($"연소 효과로 {unitName}에게 {stacks * 3} 피해");
                decrease = true;
            }

            if (decrease)
            {
                stacks--;
                if (stacks <= 0) removeList.Add(element);
                else elementStacks[element] = stacks;
            }
        }

        foreach (var e in removeList)
        {
            elementStacks.Remove(e);
            LogManager.Instance.AddLog($"{unitName}의 {e} 상태가 사라짐");
            if (CurrentElement == e) CurrentElement = State.None;
        }
    }
}


