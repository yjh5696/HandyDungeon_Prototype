using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;

public class ElementBuff
{
    public static string ApplyBuff(Character attacker, Character target, State newElement, int baseStacks)
    {
        State last = attacker.GetLastBuffElement();
        int stacks = 0;
        // 서포트 카드의 속성을 가져온다.
        if (newElement == State.None)
        {
            Debug.LogWarning("새로운 속성이 None입니다. 버프를 적용할 수 없습니다.");
            return string.Empty;
        }

        if(last == State.Recovery && newElement == State.Fervor || last == State.Fervor && newElement == State.Recovery)
        {
            attacker.AddStatusStacks(newElement, baseStacks);
            stacks = attacker.GetStatusStacks(newElement) + attacker.GetStatusStacks(last);
            target.TakeDamage(stacks * 5, target);
            stacks = Math.Abs(last - newElement);
            attacker.RemoveStatus(last);
            attacker.RemoveStatus(newElement);
            attacker.AddStatusStacks(State.Burndown, stacks);

            return $"{attacker.GetUnitName()}에게 소화 효과 발동, {newElement} 버프 {baseStacks} 스택 부여, {stacks * 5} 피해";
        }

        if (last == State.Guard && newElement == State.Gale || last == State.Gale && newElement == State.Guard)
        {
            attacker.AddStatusStacks(newElement, baseStacks);
            stacks = attacker.GetStatusStacks(newElement) + attacker.GetStatusStacks(last);
            attacker.RemoveStatus(last);
            attacker.RemoveStatus(newElement);
            attacker.AddStatusStacks(State.Vibration, stacks);

            return $"{attacker.GetUnitName()}에게 진동 효과 발동, {newElement} 버프 {baseStacks} 스택 부여";
        }

        attacker.AddStatusStacks(newElement, baseStacks);
        stacks = Mathf.Max(1, attacker.GetStatusStacks(newElement));
        attacker.SetLastBuffElement(newElement);
        if(newElement != State.None)
            return $"{attacker.GetUnitName()}에게 {newElement} 버프 {baseStacks} 스택 부여";
        else
            return string.Empty;
    }
}

