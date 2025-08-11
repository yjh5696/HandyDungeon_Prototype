using UnityEngine;

public static class ElementEffect
{
    public static float GetMultiplier(State attack, State defense) => 1f;

    public static string ApplyElementEffect(Character target, State newElement, int baseStacks)
    {
        var currentElement = target.GetCurrentElement();

        if (currentElement == newElement)
        {
            target.AddStatusStacks(newElement, baseStacks);
            return $"{newElement} 스택 {baseStacks} 증가";
        }

        if (currentElement == State.Fire && newElement == State.Wind)
        {
            int stacks = target.GetStatusStacks(State.Fire);
            target.RemoveStatus(State.Fire);
            target.SetStatus("발화", stacks / 2);
            target.TakeDamage(3 * stacks);
            return $"발화 발생! {3 * stacks} 피해";
        }
        if (currentElement == State.Wind && newElement == State.Water)
        {
            int stacks = target.GetStatusStacks(State.Wind);
            target.RemoveStatus(State.Wind);
            target.SetStatus("와류", stacks);
            target.TakeDamage(5 * stacks);
            return $"와류 발생! {5 * stacks} 피해";
        }
        if (currentElement == State.Water && newElement == State.Earth)
        {
            int stacks = target.GetStatusStacks(State.Water);
            target.RemoveStatus(State.Water);
            target.SetStatus("침전", stacks);
            target.TakeDamage(4 * stacks);
            target.ModifyDiceRoll(stacks);
            return $"침전 발생! {4 * stacks} 피해";
        }
        if (currentElement == State.Earth && newElement == State.Fire)
        {
            int stacks = target.GetStatusStacks(State.Earth);
            target.RemoveStatus(State.Earth);
            target.SetStatus("용암", stacks);
            target.TakeDamage(5 * stacks);
            target.SetExtraDamageTaken(stacks);
            return $"용암 발생! {5 * stacks} 피해";
        }

        target.SetCurrentElement(newElement);
        ApplyBaseDebuff(target, newElement, baseStacks);
        return $"{newElement} 디버프 {baseStacks} 스택 부여";
    }

    static void ApplyBaseDebuff(Character target, State element, int stacks)
    {
        switch (element)
        {
            case State.Fire: target.SetStatus("점화", stacks); break;
            case State.Wind: target.SetStatus("풍식", stacks); break;
            case State.Earth: target.SetStatus("진창", stacks); break;
            case State.Water: target.SetStatus("침식", stacks); break;
        }
    }
}


