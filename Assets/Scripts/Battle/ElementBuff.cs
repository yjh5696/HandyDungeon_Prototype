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

        attacker.AddStatusStacks(newElement, baseStacks);
        int stacks = Mathf.Max(1, attacker.GetStatusStacks(newElement));
        return $"{attacker.GetUnitName()}에게 {newElement} 버프 {baseStacks} 스택 부여";
    }
}

