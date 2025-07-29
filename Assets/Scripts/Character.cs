using UnityEngine;

public class Character
{
    protected float MaxHp; 
    protected float CurrentHp;
    protected State CurrentState;
    
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

    public void SetCurrentHp(float currentHp)
    {
        CurrentHp = currentHp;
    }

    public void SetCurrentState(State currentState)
    {
        CurrentState = currentState;
    }
    
}
