using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ElementBuff
{
    public static string ApplyBuff(Character attacker, State newElement, int baseStacks)
    {
        // 서포트 카드의 속성을 가져온다.
        if(newElement == State.None)
        {
            Debug.LogWarning("새로운 속성이 None입니다. 버프를 적용할 수 없습니다.");
            return string.Empty;
        }
        else if(newElement == State.Fire)
        {
            attacker.AddStatusStacks(State.Fervor, baseStacks);
            int stacks = Mathf.Max(1, attacker.GetStatusStacks(State.Fervor));
            attacker.SetFervorDamage(stacks);
            return $"{attacker.GetUnitName()}에게 불 속성 버프가 적용되었습니다.";
        }
        else if(newElement == State.Water)
        {
            attacker.AddStatusStacks(State.Recovery, baseStacks);
            int stacks = Mathf.Max(1, attacker.GetStatusStacks(State.Recovery));
            //attacker.SetRecovery(stacks, diceValue, damage);
            return $"{attacker.GetUnitName()}에게 물 속성 버프가 적용되었습니다.";
        }
        else if(newElement == State.Wind)
        {
            attacker.AddStatusStacks(State.Gale, baseStacks);
            int stacks = Mathf.Max(1, attacker.GetStatusStacks(State.Gale));
            return $"{attacker.GetUnitName()}에게 바람 속성 버프가 적용되었습니다.";
        }
        else if(newElement == State.Earth)
        {
            attacker.AddStatusStacks(State.Guard, baseStacks);
            int stacks = Mathf.Max(1, attacker.GetStatusStacks(State.Guard));
            return $"{attacker.GetUnitName()}에게 땅 속성 버프가 적용되었습니다.";
        }

        return $"{newElement} 버프 {baseStacks} 스택 부여";
    }
}

