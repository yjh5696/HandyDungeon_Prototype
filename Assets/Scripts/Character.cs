using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour
{
    protected float MaxHp; 
    protected float CurrentHp;
    protected State CurrentState;
    protected HPBar HpBar;
    public event EventHandler OnHpChanged;

    private void Start()
    {
        OnHpChanged += HpChanged;
    }

    public float GetMaxHp()
    {
        return MaxHp;
    }

    public float GetCurrentHp()
    {
        return CurrentHp;
    }

    public State GetCurrentState()
    {
        return CurrentState;
    }
    
    public void SetMaxHp(float maxHp)
    {
        MaxHp = maxHp;
    }

    public void SetHpBar(HPBar hpBar)
    {
        HpBar = hpBar;
    }

    public void SetCurrentHp(float currentHp)
    {
        CurrentHp = currentHp;
        OnHpChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetCurrentState(State currentState)
    {
        CurrentState = currentState;
    }

    protected virtual void HpChanged(object sender, EventArgs e)
    {
        HpBar.HpChanged(CurrentHp, MaxHp);
    }
}
