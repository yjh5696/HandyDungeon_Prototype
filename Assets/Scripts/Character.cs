using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    public string unitName;
    protected float MaxHp;
    protected float CurrentHp;
    protected State CurrentState;
    protected HPBar HpBar;
    public event EventHandler OnHpChanged;
    public int nextTurnDiceBonus = 0;
    public int nextTurnDiceMultiplier = 1;

    protected State CurrentElement = State.None;
    protected Dictionary<State, int> elementStacks = new Dictionary<State, int>();
    protected int extraDamageStacks = 0;
    protected int fervorDamage = 0;
    protected float RecoveryValue = 0;
    protected float Recovery = 0;
    protected float ShieldValue = 0;
    protected float Shield = 0;

    [SerializeField] int maxElementStack = 10;

    // 최근 맞은 속성 기록 (유지턴 제거)
    protected State lastElement = State.None;
    protected State lastBuffElement = State.None;

    public State GetLastElement() => lastElement;
    public State GetLastBuffElement() => lastBuffElement;
    public State GetCurrentElement() => CurrentElement;

    public void SetLastElement(State element)
    {
        Debug.Log($"[SetLastElement] {unitName} : {lastElement} → {element}");
        lastElement = element;
    }

    public void SetLastBuffElement(State element)
    {
        Debug.Log($"[SetLastBuffElement] {unitName} : {lastBuffElement} → {element}");
        lastBuffElement = element;
    }

    [SerializeField] private List<CardDataSO> cards; // 인스펙터에서 직접 카드 풀 할당 가능
    public List<CardDataSO> Cards{ get => cards; protected set => cards = value; } // 외부에서 읽기 전용으로 노출
    protected CardDataSO currentCard;

    public virtual void DrawAndUseCard()
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning($"{name} 카드 덱이 없습니다.");
            return;
        }
        int result = Random.Range(0, cards.Count);
        currentCard = cards[result];
        CardManager.Instance.selectedCard = currentCard;
        LogManager.Instance.AddDelayedLog($"{name}이/가 {currentCard.C_Name}을 사용했습니다.", 1);
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

    // 상태와 스택 수를 쌍으로 반환하는 함수
    public Dictionary<State, int> GetCurrentStatesWithStacks()
    {
        // elementStacks는 Dictionary<State, int>로 상태와 스택수를 저장 중이므로 복사본 반환 가능
        return new Dictionary<State, int>(elementStacks);
    }

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
        {
            elementStacks[element] = Mathf.Min(elementStacks[element] + stacks, maxElementStack);
        }
        else
        {
            elementStacks[element] = Mathf.Min(stacks, maxElementStack);
        }

        CurrentElement = element;

        if (element == State.Fire || element == State.Water || element == State.Air || element == State.Land)
            SetLastElement(element); // 즉시 반영
        else if (element == State.Fervor || element == State.Gale || element == State.Guard || element == State.Recovery)
            SetLastBuffElement(element); // 즉시 반영
    }

    public void SetStatus(string statusName, int stacks)
    {
        var mapped = MapStatusNameToState(statusName);
        elementStacks[mapped] = Mathf.Min(stacks, maxElementStack);  // 최대 10으로 제한
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
            "풍식" => State.Air,
            "진창" => State.Land,
            "침식" => State.Water,
            "연소" => State.Ignition,
            "열정" => State.Fervor,
            "질풍" => State.Gale,
            "수호" => State.Guard,
            "재생" => State.Recovery,
            //"보호막" => State.Shield,
            //"회복" => State.Healing,
            "진동" => State.Vibration,
            _ => State.None
        };
    }

    // 교란: 홀짝 조건부 공격력 감소
    public float SetAirEffect(float baseDamage, int diceValue)
    {
        if (elementStacks.ContainsKey(State.Air))
        {
            int stacks = elementStacks[State.Air];
            if ((stacks % 2) == (diceValue % 2))
            {
                float original = baseDamage;
                baseDamage = Mathf.Max(0, baseDamage - stacks * 2f);
                LogManager.Instance.AddDelayedLog($"교란 효과로 공격력 {original} → {baseDamage} 감소", 1);
            }
        }
        return baseDamage;
    }

    // 진동: 공격 시 피해 반사
    public void SetVibrationEffect(float totalDamage, Character attacker)
    {
        if (elementStacks.ContainsKey(State.Vibration))
        {
            totalDamage = Mathf.Round(totalDamage);
            attacker.HitDamage(totalDamage);
            LogManager.Instance.AddDelayedLog($"{unitName}의 진동 효과로 {attacker.GetUnitName()}에게 {totalDamage} 피해", 1);
            if (attacker is Player)
            {
                PlayerManager.Instance.PlayHitAnimation();
            }
            else if (attacker is Enemy)
            {
                EnemyManager.Instance.EnemyHitAnimation();
            }
        }
    }
    // 소화 : 공격 시 피해 감소
    public float SetBurndownEffect(float totalDamage)
    {
        if (elementStacks.ContainsKey(State.Burndown))
        {
            totalDamage /= 2; // 버닝 효과로 피해 절반 감소
            totalDamage = Mathf.Round(totalDamage);
            elementStacks[State.Burndown] = Mathf.Max(0, elementStacks.GetValueOrDefault(State.Burndown, 0) - 1);
            LogManager.Instance.AddDelayedLog($"{unitName}의 소화 효과로 피해 {totalDamage} 감소", 1);
        }
        return totalDamage;
    }

    // 소화 상태 제거
    public void SetBurndownClear()
    {
        if (elementStacks.ContainsKey(State.Burndown))
        {
            LogManager.Instance.AddDelayedLog($"{unitName}의 소화 상태가 사라짐", 1);
            RemoveStatus(State.Burndown);
        }
    }

    // 순풍: 홀짝 조건부 공격력 증가
    public float SetGaleEffect(float baseDamage, int diceValue)
    {
        if(elementStacks.ContainsKey(State.Gale))
        {
            int stacks = elementStacks[State.Gale];
            if ((stacks % 2) == (diceValue % 2))
            {
                float original = baseDamage;
                baseDamage += stacks * 2f; // 순풍 효과로 공격력 증가
                baseDamage = Mathf.Round(baseDamage);
                LogManager.Instance.AddDelayedLog($"순풍 효과로 공격력 {original} → {baseDamage} 증가", 1);
            }
        }
        return baseDamage;
    }

    // 균열: 발동 시 스택 감소
    public float SetLandEffect(float damage)
    {
        if (elementStacks.ContainsKey(State.Land))
        {
            int stacks = elementStacks[State.Land];
            float original = damage;
            damage *= 1f + stacks * 0.05f;
            damage = Mathf.Round(damage);

            if (Mathf.Abs(original - damage) > 0.001f)
            {
                LogManager.Instance.AddDelayedLog($"균열 효과로 피해 {original} → {damage} 증가", 1);
            }
        }
        return damage;
    }

    // 수호: 발동 시 피해 감소
    public float SetGuardEffect(float Damage)
    {
        if(elementStacks.ContainsKey(State.Guard))
        {
            int stacks = elementStacks[State.Guard];
            float original = Damage;
            Damage *= 1f - stacks * 0.05f;
            Damage = Mathf.Round(Damage);

            if (Mathf.Abs(original - Damage) > 0.001f)
            {
                LogManager.Instance.AddDelayedLog($"수호 효과로 피해 {original} → {Damage} 감소", 1);
            }
        }
        return Damage;
    }

    // 젖음: 발동 시 스택 감소
    public float SetWaterEffect(float baseDamage, Character target)
    {
        if (elementStacks.ContainsKey(State.Water))
        {
            int stack = elementStacks[State.Water];

            float newHp = Mathf.Min(target.GetMaxHp(), target.GetCurrentHp() + stack);
            target.SetCurrentHp(newHp);
            LogManager.Instance.AddDelayedLog($"{unitName}이/가 젖음 효과로 {stack} 회복", 1);

            baseDamage = Mathf.Max(0, baseDamage - stack);
            LogManager.Instance.AddDelayedLog($"{unitName}의 젖음 효과로 {stack}만큼 데미지 감소", 1);
        }
        return baseDamage;
    }

    // 데미지 처리
    public void TakeDamage(float damage, Character target)
    {
        if (extraDamageStacks > 0) damage += extraDamageStacks;
        damage = Mathf.Round(damage);
        if (Shield > 0)
        {
            if (damage <= Shield)
            {
                Shield -= damage;
                Shield = Mathf.Round(Shield);
                LogManager.Instance.AddDelayedLog($"{unitName}이/가 보호막으로 {damage} 피해 방어 (남은 보호막: {Shield})", 1);
                damage = 0f;
            }
        }
        float clearHp = GetCurrentHp() - damage;
        clearHp = Mathf.Round(clearHp);
        SetCurrentHp(clearHp);

        if (target.GetCurrentHp() <= 0)
        {
            if (target is Player player)
            {
                player.PlayerDie();
            }
            else if (target is Enemy enemy)
            {
                enemy.EnemyDie();
            }
            return;
        }
        if(PlayerManager.Instance.Player.GetCurrentHp() <= 0)
        {
            PlayerManager.Instance.Player.PlayerDie();
            return;
        }
        else if(EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
        {
            EnemyManager.Instance.Enemy.EnemyDie();
            return;
        }
    }
    
    // 데미지 처리
    public void HitDamage(float damage)
    {
        damage = Mathf.Round(damage);
        if (Shield > 0)
        {
            if (damage <= Shield)
            {
                Shield -= damage;
                Shield = Mathf.Round(Shield);
                LogManager.Instance.AddDelayedLog($"{unitName}이/가 보호막으로 {damage} 피해 방어 (남은 보호막: {Shield})", 1);
                damage = 0f;
            }
        }
        float clearHp = GetCurrentHp() - damage;
        clearHp = Mathf.Round(clearHp);
        SetCurrentHp(clearHp);

        if (PlayerManager.Instance.Player.GetCurrentHp() <= 0)
        {
            PlayerManager.Instance.Player.PlayerDie();
            return;
        }
        else if (EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
        {
            EnemyManager.Instance.Enemy.EnemyDie();
            return;
        }
    }
    protected virtual void OnDeath() { }
    public void SetExtraDamageTaken(int stacks) => extraDamageStacks = stacks;

    // 열정 효과
    public int SetFervorDamage()
    {
        if(elementStacks.ContainsKey(State.Fervor))
        {
            int stacks = GetStatusStacks(State.Fervor);
            fervorDamage += stacks;
            LogManager.Instance.AddDelayedLog($"{unitName}의 열정 효과로 {stacks} 데미지 추가", 1);

        }
        return fervorDamage;
    }

    // 재생 효과
    public void SetRecovery(int diceValue, string cardType)
    {
        if(elementStacks.ContainsKey(State.Recovery))
        {
            int stacks = elementStacks[State.Recovery];
            if(cardType == "Heal")
            {
                SetHealEffect(stacks, diceValue); // 회복 설정
            }
            else if(cardType == "Sheild")
            {
                SetShieldEffect(stacks, diceValue); // 보호막 설정
            }
        }
    }

    // 보호막 효과
    public void SetShieldEffect(int stacks, int diceValue)
    {
        if(diceValue <= 0)
        {
            diceValue = 1;
        }
        ShieldValue = stacks;
        Shield += (ShieldValue * diceValue) / 3;
        Shield = Mathf.Round(Shield);
        LogManager.Instance.AddDelayedLog($"{unitName}이/가 보호막 효과로{(ShieldValue * diceValue) / 3} 보호막 획득 (총 보호막: {Shield})", 1);
    }
    
    // 회복 효과
    public void SetHealEffect(int stacks, int diceValue)
    {
        if (diceValue <= 0)
        {
            diceValue = 1;
        }
        RecoveryValue = stacks;
        Recovery = (RecoveryValue * diceValue) / 3;
        Recovery = Mathf.Round(Recovery);
        SetCurrentHp(CurrentHp + Recovery);
        LogManager.Instance.AddDelayedLog($"{unitName}이/가 재생 효과로 {Recovery} 회복", 1);
    }

    public void SetShield(float damage)
    {
        Shield += damage;
        Shield = Mathf.Round(Shield);
        LogManager.Instance.AddDelayedLog($"{unitName}이/가 {damage} 보호막 획득 (총 보호막: {Shield})", 1);
    }

    public void SetHeal(float damage)
    {
        damage = Mathf.Round(damage);
        SetCurrentHp(CurrentHp + damage);
        CurrentHp = Mathf.Round(CurrentHp);
        LogManager.Instance.AddDelayedLog($"{unitName}이/가 {damage} 회복 (현재 체력: {CurrentHp}/{MaxHp})", 1);
    }

    //(본인 턴 종료 시 스택 감소)
    public virtual void OnTurnEnd_WindDecrease()
    {
        // 젖음(Water) 상태 처리
        if (elementStacks.ContainsKey(State.Water))
        {
            int stacks = elementStacks[State.Water] - 1;
            if (stacks <= 0)
            {

                LogManager.Instance.AddDelayedLog($"{unitName}의 물 상태가 사라짐", 1);
                if (CurrentElement == State.Water) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Water] = stacks;
                LogManager.Instance.AddDelayedLog($"{unitName}의 물 스택 1 감소 (남은 스택: {stacks})", 1);
            }
        }

        // 교란 (Air) 상태 처리
        if (elementStacks.ContainsKey(State.Air))
        {
            int stacks = elementStacks[State.Air] - 1;
            if (stacks <= 0)
            {
                LogManager.Instance.AddDelayedLog($"{unitName}의 교란 상태가 사라짐", 1);
                if (CurrentElement == State.Air) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Air] = stacks;
                LogManager.Instance.AddDelayedLog($"{unitName}의 교란 스택 1 감소 (남은 스택: {stacks})", 1);
            }
        }

        // 균열(Land) 상태 처리
        if (elementStacks.ContainsKey(State.Land))
        {
            int stacks = elementStacks[State.Land] - 1;
            if (stacks <= 0)
            {
                LogManager.Instance.AddDelayedLog($"{unitName}의 대지 상태가 사라짐", 1);
                if (CurrentElement == State.Land) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Land] = stacks;
                LogManager.Instance.AddDelayedLog($"{unitName}의 대지 스택 1 감소 (남은 스택: {stacks})", 1);
            }
        }

        // 열정(Fervor) 상태 처리
        if (elementStacks.ContainsKey(State.Fervor))
        {
            int stacks = elementStacks[State.Fervor] - 1;
            if (stacks <= 0)
            {
                LogManager.Instance.AddDelayedLog($"{unitName}의 페버 상태가 사라짐", 1);
                if (CurrentElement == State.Fervor) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Fervor] = stacks;
                LogManager.Instance.AddDelayedLog($"{unitName}의 페버 스택 1 감소 (남은 스택: {stacks})", 1);
            }
        }

        // 순풍(Gale) 상태 처리
        if (elementStacks.ContainsKey(State.Gale))
        {
            int stacks = elementStacks[State.Gale] - 1;
            if (stacks <= 0)
            {
                LogManager.Instance.AddDelayedLog($"{unitName}의 질풍 상태가 사라짐", 1);
                if (CurrentElement == State.Gale) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Gale] = stacks;
                LogManager.Instance.AddDelayedLog($"{unitName}의 질풍 스택 1 감소 (남은 스택: {stacks})", 1);
            }
        }
        // 수호(Guard) 상태 처리
        if (elementStacks.ContainsKey(State.Guard))
        {
            int stacks = elementStacks[State.Guard] - 1;
            if (stacks <= 0)
            {
                LogManager.Instance.AddDelayedLog($"{unitName}의 수호 상태가 사라짐", 1);
                if (CurrentElement == State.Guard) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Guard] = stacks;
                LogManager.Instance.AddDelayedLog($"{unitName}의 수호 스택 1 감소 (남은 스택: {stacks})", 1);
            }
        }
        // 재생(Recovery) 상태 처리
        if (elementStacks.ContainsKey(State.Recovery))
        {
            int stacks = elementStacks[State.Recovery] - 1;
            if (stacks <= 0)
            {
                LogManager.Instance.AddDelayedLog($"{unitName}의 재생 상태가 사라짐", 1);
                if (CurrentElement == State.Recovery) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Recovery] = stacks;
                LogManager.Instance.AddDelayedLog($"{unitName}의 재생 스택 1 감소 (남은 스택: {stacks})", 1);
            }
        }
        // 진동 상태 처리
        if (elementStacks.ContainsKey(State.Vibration))
        {
            int stacks = elementStacks[State.Vibration] - 1;
            if (stacks <= 0)
            {
                LogManager.Instance.AddDelayedLog($"{unitName}의 진동 상태가 사라짐", 1);
                if (CurrentElement == State.Vibration) CurrentElement = State.None;
            }
            else
            {
                elementStacks[State.Vibration] = stacks;
                LogManager.Instance.AddDelayedLog($"{unitName}의 진동 스택 1 감소 (남은 스택: {stacks})", 1);
            }
        }
    }


    // 디버프 스택 초기화
    public void ClearDebuffStacks()
    {
        // 디버프 상태들(State.Fire, Water, Air, Land)만 따로 관리
        State[] debuffs = new State[] { State.Fire, State.Water, State.Air, State.Land, State.Ignition };
        foreach (var debuff in debuffs)
        {
            if (elementStacks.ContainsKey(debuff))
                elementStacks.Remove(debuff);
        }

        // CurrentElement가 디버프 상태면 초기화
        if (Array.Exists(debuffs, d => d == CurrentElement))
            CurrentElement = State.None;

        LogManager.Instance.AddDelayedLog($"{unitName}의 디버프 스택이 초기화되었습니다.", 1);
    }

    // 다음 턴 주사위 보너스
    public void AddNextTurnDiceBouns(int bonus)
    {
        nextTurnDiceBonus += bonus;
        Debug.Log($"[AddNextTurnDiceBouns] {unitName} : nextTurnDiceBonus :+{nextTurnDiceBonus}");
        LogManager.Instance.AddDelayedLog($"{unitName}의 다음 턴 주사위 값이 {bonus}만큼 증가합니다.", 1);
    }

    // 다음 턴 주사위 2배
    public void NextTurnDiceMultiplier(int multiplier)
    {
        nextTurnDiceMultiplier *= multiplier;
        Debug.Log($"[NextTurnDiceMultiplier] {unitName} : nextTurnDiceMultiplier :{nextTurnDiceMultiplier}배");
        LogManager.Instance.AddDelayedLog($"{unitName}의 다음 턴 주사위 값이 {multiplier}배로 증가합니다.", 1);
    }

    public void ClearDiceBouns()
    {
        nextTurnDiceBonus = 0;
        nextTurnDiceMultiplier = 1;
    }

    // DOT 처리 (점화, 연소)
    public virtual void ProcessEndTurnEffects(Character attacker)
    {
        List<State> removeList = new List<State>();

        foreach (var kvp in new Dictionary<State, int>(elementStacks))
        {
            State element = kvp.Key;
            int stacks = kvp.Value;
            bool decrease = false;

            if (element == State.Fire)
            {
                TakeDamage(stacks, attacker);
                LogManager.Instance.AddDelayedLog($"점화 효과로 {unitName}에게 {stacks} 피해", 1);
                decrease = true;
            }
            else if (element == State.Ignition)
            {
                TakeDamage(stacks * 3, attacker);
                LogManager.Instance.AddDelayedLog($"연소 효과로 {unitName}에게 {stacks * 3} 피해", 1);
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
            LogManager.Instance.AddDelayedLog($"{unitName}의 {e} 상태가 사라짐", 1);
        }
    }
}


