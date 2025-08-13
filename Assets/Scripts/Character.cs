using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string unitName;
    protected float MaxHp;
    protected float CurrentHp;
    protected State CurrentState;
    protected HPBar HpBar;
    public event EventHandler OnHpChanged;

    protected State CurrentElement = State.None;
    protected Dictionary<State, int> elementStacks = new Dictionary<State, int>();
    protected int extraDamageStacks = 0;

    // 최근 맞은 속성 기록 (유지턴 제거)
    protected State lastElement = State.None;

    public State GetLastElement() => lastElement;
    public State GetCurrentElement() => CurrentElement;

    public void SetLastElement(State element)
    {
        Debug.Log($"[SetLastElement] {unitName} : {lastElement} → {element}");
        lastElement = element;
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
            elementStacks[element] += stacks;
        else
            elementStacks[element] = stacks;

        CurrentElement = element;
        SetLastElement(element); // 속성 부여 시 즉시 반영
    }

    public void SetStatus(string statusName, int stacks)
    {
        var mapped = MapStatusNameToState(statusName);
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
            "풍식" => State.Wind,
            "진창" => State.Earth,
            "침식" => State.Water,
            "발화" => State.Ignition,
            _ => State.None
        };
    }

    // 풍식: 홀짝 조건부 공격력 감소
    public float ModifyOutgoingDamage(float baseDamage, int diceValue)
    {
        if (elementStacks.ContainsKey(State.Wind))
        {
            int stacks = elementStacks[State.Wind];
            if ((stacks % 2) == (diceValue % 2))
            {
                float original = baseDamage;
                baseDamage = Mathf.Max(0, baseDamage - stacks * 2f);
                LogManager.Instance.AddLog($"풍식 효과로 공격력 {original} → {baseDamage} 감소");
            }
        }
        return baseDamage;
    }

    // 진창: 발동 시 스택 감소
    public float ModifyIncomingDamage(float damage)
    {
        if (elementStacks.ContainsKey(State.Earth))
        {
            int stacks = elementStacks[State.Earth];
            float original = damage;
            damage *= 1f + stacks * 0.1f;

            if (Mathf.Abs(original - damage) > 0.001f)
            {
                LogManager.Instance.AddLog($"진창 효과로 피해 {original} → {damage} 증가");
                stacks--;
                if (stacks <= 0)
                {
                    elementStacks.Remove(State.Earth);
                    LogManager.Instance.AddLog($"{unitName}의 진창 상태가 사라짐");
                    if (CurrentElement == State.Earth) CurrentElement = State.None;
                }
                else elementStacks[State.Earth] = stacks;
            }
        }
        return damage;
    }

    // 침식: 발동 시 스택 감소
    public int ModifyDiceRoll(int diceValue)
    {
        if (elementStacks.ContainsKey(State.Water))
        {
            int stacks = elementStacks[State.Water];
            int original = diceValue;
            diceValue = Mathf.Max(0, diceValue - stacks);

            if (diceValue != original)
            {
                LogManager.Instance.AddLog($"침식 효과로 주사위 눈 {original} → {diceValue} 감소");
                stacks--;
                if (stacks <= 0)
                {
                    elementStacks.Remove(State.Water);
                    LogManager.Instance.AddLog($"{unitName}의 침식 상태가 사라짐");
                    if (CurrentElement == State.Water) CurrentElement = State.None;
                }
                else elementStacks[State.Water] = stacks;
            }
        }
        return diceValue;
    }

    public void TakeDamage(float amount)
    {
        if (extraDamageStacks > 0) amount += extraDamageStacks;
        SetCurrentHp(CurrentHp - amount);
        if (CurrentHp <= 0) OnDeath();
    }
    protected virtual void OnDeath() { }
    public void SetExtraDamageTaken(int stacks) => extraDamageStacks = stacks;

    // 풍식 감소 (본인 턴 종료 시)
    public virtual void OnTurnEnd_WindDecrease()
    {
        if (elementStacks.ContainsKey(State.Wind))
        {
            int stacks = elementStacks[State.Wind] - 1;
            if (stacks <= 0)
            {
                elementStacks.Remove(State.Wind);
                LogManager.Instance.AddLog($"{unitName}의 풍식 상태가 사라짐");
                if (CurrentElement == State.Wind) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Wind] = stacks;
                LogManager.Instance.AddLog($"{unitName}의 풍식 스택 1 감소 (남은 스택: {stacks})");
            }
        }
    }

    // DOT 처리 (점화, 발화)
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
                LogManager.Instance.AddLog($"발화 효과로 {unitName}에게 {stacks * 3} 피해");
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


