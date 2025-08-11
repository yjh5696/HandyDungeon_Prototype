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

    public void SetUnitName(string name) => unitName = name;
    public string GetUnitName() => unitName;

    private void Start() => OnHpChanged += HpChanged;

    public float GetMaxHp() => MaxHp;
    public float GetCurrentHp() => CurrentHp;
    public State GetCurrentState() => CurrentState;

    public void SetMaxHp(float maxHp) => MaxHp = maxHp;
    public void SetHpBar(HPBar hpBar) => HpBar = hpBar;

    public void SetCurrentHp(float currentHp)
    {
        CurrentHp = Mathf.Clamp(currentHp, 0, MaxHp);
        OnHpChanged?.Invoke(this, EventArgs.Empty);
    }
    public void SetCurrentState(State currentState) => CurrentState = currentState;

    protected virtual void HpChanged(object sender, EventArgs e) =>
        HpBar.HpChanged(CurrentHp, MaxHp);

    public State GetCurrentElement() => CurrentElement;
    public void SetCurrentElement(State element) => CurrentElement = element;

    public void AddStatusStacks(State element, int stacks)
    {
        if (elementStacks.ContainsKey(element))
            elementStacks[element] += stacks;
        else
            elementStacks[element] = stacks;
        CurrentElement = element;
    }

    public int GetStatusStacks(State element) =>
        elementStacks.ContainsKey(element) ? elementStacks[element] : 0;

    public void RemoveStatus(State element)
    {
        if (elementStacks.ContainsKey(element))
            elementStacks.Remove(element);
        if (CurrentElement == element) CurrentElement = State.None;
    }

    public void SetStatus(string statusName, int stacks)
    {
        Debug.Log($"{unitName}에게 {statusName} 상태 {stacks} 스택 부여");
        State mapped = MapStatusNameToState(statusName);
        elementStacks[mapped] = stacks;
        CurrentElement = mapped;
    }

    State MapStatusNameToState(string statusName)
    {
        switch (statusName)
        {
            case "점화": return State.Fire;
            case "풍식": return State.Wind;
            case "진창": return State.Earth;
            case "침식": return State.Water;
            default: return State.None;
        }
    }

    public float ModifyOutgoingDamage(float baseDamage)
    {
        if (elementStacks.ContainsKey(State.Wind))
        {
            int stacks = elementStacks[State.Wind];
            baseDamage = Mathf.Max(0, baseDamage - stacks * 2f);
        }
        return baseDamage;
    }

    public float ModifyIncomingDamage(float damage)
    {
        if (elementStacks.ContainsKey(State.Earth))
        {
            int stacks = elementStacks[State.Earth];
            damage *= 1f + stacks * 0.1f;
        }
        return damage;
    }

    public int ModifyDiceRoll(int diceValue)
    {
        if (elementStacks.ContainsKey(State.Water))
        {
            int stacks = elementStacks[State.Water];
            diceValue = Mathf.Max(1, diceValue - stacks);
        }
        return diceValue;
    }

    public void TakeDamage(float amount)
    {
        if (extraDamageStacks > 0) amount += extraDamageStacks;
        SetCurrentHp(CurrentHp - amount);
        if (GetCurrentHp() <= 0) OnDeath();
    }

    protected virtual void OnDeath() { }
    public void SetExtraDamageTaken(int stacks) => extraDamageStacks = stacks;
    public virtual void ApplyStatusEffect(State state) { }

    public virtual void ProcessEndTurnEffects()
    {
        List<State> removeList = new List<State>();

        foreach (var kvp in new Dictionary<State, int>(elementStacks))
        {
            var element = kvp.Key;
            int stacks = kvp.Value;

            if (element == State.Fire) TakeDamage(stacks);

            stacks--;
            if (stacks <= 0) removeList.Add(element);
            else elementStacks[element] = stacks;
        }

        foreach (var e in removeList)
        {
            elementStacks.Remove(e);
            if (CurrentElement == e) CurrentElement = State.None;
        }
    }
}


