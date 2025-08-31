using Unity.VisualScripting;
using UnityEngine;

public static class ElementEffect
{
    public static float GetMultiplier(State attack, State defense) => 1f;

    public static string ApplyElementEffect(Character attacker, Character target, State newElement, int baseStacks)
    {
        State last = target.GetLastElement();
        Debug.Log($"[ElementEffect] {target.GetUnitName()} last={last}, new={newElement}");

        // 동일 속성 = 스택 증가
        if (last == newElement)
        {
            target.AddStatusStacks(newElement, baseStacks);
            target.SetLastElement(newElement);
            return $"{target.GetUnitName()}에게 {newElement} 스택 {baseStacks} 증가";
        }

        // 발화
        if (last == State.Fire && newElement == State.Air)
        {
            int stacks = Mathf.Max(1, target.GetStatusStacks(State.Fire));
            target.RemoveStatus(State.Fire);
            target.SetStatus("연소", (stacks + 1) / 2);
            ApplyBaseDebuff(target, State.Air, baseStacks);
            target.SetLastElement(State.Air);
            return $"연소 발생! 턴마다 {3 * ((stacks + 1) / 2)} 피해 + 교란 {baseStacks} 스택 부여";
        }

        // 와류
        if (last == State.Air && newElement == State.Water)
        {
            int stacks = Mathf.Max(1, target.GetStatusStacks(State.Air));
            target.RemoveStatus(State.Air);
            target.SetStatus("동상", (stacks + 1));
            target.TakeDamage(5 * stacks, target);
            ApplyBaseDebuff(target, State.Water, baseStacks);
            target.SetLastElement(State.Water);
            return $"동상 발생! {5 * stacks} 피해 + 젖음 {baseStacks} 스택 부여";
        }

        // 암반화
        if (last == State.Water && newElement == State.Land)
        {
            int stacks = Mathf.Max(1, target.GetStatusStacks(State.Water));
            target.RemoveStatus(State.Water);
            target.SetStatus("암반화", (stacks + 1));
            target.TakeDamage(4 * stacks, target);
            target.SetShieldEffect(2 * stacks, -1);
            ApplyBaseDebuff(target, State.Land, baseStacks);
            target.SetLastElement(State.Land);
            return $"암반화 발생! {4 * stacks} 피해 + 균열 {baseStacks} 스택 부여";
        }

        // 분화
        if (last == State.Land && newElement == State.Fire)
        {
            int stacks = Mathf.Max(1, target.GetStatusStacks(State.Land));
            float mult = 1f + stacks * 0.1f;
            target.RemoveStatus(State.Land);
            target.SetStatus("분화", (stacks + 1));
            target.TakeDamage(5 * stacks * mult, target);
            target.SetExtraDamageTaken(stacks);
            ApplyBaseDebuff(target, State.Fire, baseStacks);
            target.SetLastElement(State.Fire);
            return $"분화 발생! {5 * stacks * mult} 피해 + 점화 {baseStacks} 스택 부여";
        }

        // 기본 디버프
        target.AddStatusStacks(newElement, baseStacks);
        target.SetLastElement(newElement);
        if(newElement != State.None)
        {
            return $"{target.GetUnitName()}에게 {newElement} 디버프 {baseStacks} 스택 부여";
        }
        else
        {
            return string.Empty;
        }
    }

    static void ApplyBaseDebuff(Character target, State element, int stacks)
    {
        switch (element)
        {
            case State.Fire: target.SetStatus("점화", stacks); break;
            case State.Air: target.SetStatus("교란", stacks); break;
            case State.Land: target.SetStatus("균열", stacks); break;
            case State.Water: target.SetStatus("젖음", stacks); break;
        }
    }
}



