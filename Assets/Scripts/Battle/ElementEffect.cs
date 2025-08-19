using UnityEngine;

public static class ElementEffect
{
    public static float GetMultiplier(State attack, State defense) => 1f;

    public static string ApplyElementEffect(Character target, State newElement, int baseStacks)
    {
        State last = target.GetLastElement();
        Debug.Log($"[ElementEffect] {target.GetUnitName()} last={last}, new={newElement}");

        // 동일 속성 = 스택 증가
        if (last == newElement)
        {
            target.AddStatusStacks(newElement, baseStacks);
            target.SetLastElement(newElement);
            return $"{newElement} 스택 {baseStacks} 증가";
        }

        // 발화
        if (last == State.Fire && newElement == State.Wind)
        {
            int stacks = Mathf.Max(1, target.GetStatusStacks(State.Fire));
            target.RemoveStatus(State.Fire);
            target.SetStatus("발화", (stacks + 1) / 2);
            ApplyBaseDebuff(target, State.Wind, baseStacks);
            target.SetLastElement(State.Wind);
            return $"발화 발생! 턴마다 {3 * (stacks / 2)} 피해 + 풍식 {baseStacks} 스택 부여";
        }

        // 와류
        if (last == State.Wind && newElement == State.Water)
        {
            int stacks = Mathf.Max(1, target.GetStatusStacks(State.Wind));
            target.RemoveStatus(State.Wind);
            target.SetStatus("와류", (stacks + 1));
            target.TakeDamage(5 * stacks);
            ApplyBaseDebuff(target, State.Water, baseStacks);
            target.SetLastElement(State.Water);
            return $"와류 발생! {5 * stacks} 피해 + 침식 {baseStacks} 스택 부여";
        }

        // 침전
        if (last == State.Water && newElement == State.Earth)
        {
            int stacks = Mathf.Max(1, target.GetStatusStacks(State.Water));
            target.RemoveStatus(State.Water);
            target.SetStatus("침전", (stacks + 1));
            target.TakeDamage(4 * stacks);
            target.SetWaterEffect(stacks);
            ApplyBaseDebuff(target, State.Earth, baseStacks);
            target.SetLastElement(State.Earth);
            return $"침전 발생! {4 * stacks} 피해 + 진창 {baseStacks} 스택 부여";
        }

        // 용암
        if (last == State.Earth && newElement == State.Fire)
        {
            int stacks = Mathf.Max(1, target.GetStatusStacks(State.Earth));
            float mult = 1f + stacks * 0.1f;
            target.RemoveStatus(State.Earth);
            target.SetStatus("용암", (stacks + 1));
            target.TakeDamage(5 * stacks * mult);
            target.SetExtraDamageTaken(stacks);
            ApplyBaseDebuff(target, State.Fire, baseStacks);
            target.SetLastElement(State.Fire);
            return $"용암 발생! {5 * stacks * mult} 피해 + 점화 {baseStacks} 스택 부여";
        }

        // 기본 디버프
        target.AddStatusStacks(newElement, baseStacks);
        target.SetLastElement(newElement);
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



