using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Attack_Button_DiceRoll : MonoBehaviour
{
    public static Attack_Button_DiceRoll Instance;
    public DiceRoll diceRoll;
    [SerializeField] private float switchTurnDelay;
    [SerializeField] private float showDiceResultTime;
    [SerializeField] private Button diceButton;

    CardDataSO selectedCard = null;
    CardDataSO SpecialCard = null;

    private bool isDiceRolling = false;

    private void Awake()
    {
        Instance = this;
    }

    private async void OnEnable()
    {
        SpecialCard = CardManager.Instance.selectedCard;
        diceButton.interactable = true;
        if (SpecialCard != null && SpecialCard.C_Type == "Special" && GameManager.Instance.isPlayerTurn)
        {
            int fakeDiceValue = 0;
            await ShowDiceResultWithDelayAsync(0, fakeDiceValue);
        }
    }

    public void OnAttackButtonClicked()
    {
        if (SpecialCard != null && SpecialCard.C_Type == "Special" && GameManager.Instance.isPlayerTurn)
        {
            Debug.Log("플레이어 특수 카드 사용 - 주사위 굴리지 않음");
            return;
        }

        if (isDiceRolling)
        {
            // 주사위가 굴러가는 중에 버튼 다시 눌리면 즉시 결과 고정
            diceRoll.ForceFinishRoll(OnDiceRolled);
            diceButton.interactable = false;
            return;
        }

        isDiceRolling = true;
        diceRoll.RollDice(OnDiceRolled);
        LogManager.Instance.AddLog("");
        if (GameManager.Instance.isPlayerTurn)
        {
            LogManager.Instance.AddLog("주사위를 굴렸습니다!");
        }
        else
        {
            string enemyName = EnemyManager.Instance.Enemy.GetEnemySo().EnemyName;
            LogManager.Instance.AddLog($"{enemyName}이/가 주사위를 굴렸습니다!");
        }
    }

    private async void OnDiceRolled(int value)
    {
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog($"주사위 눈이 {value}가/이 나왔습니다!");
        Debug.Log($"주사위 눈이 {value}가/이 나왔습니다!");
        isDiceRolling = false;
        await ShowDiceResultWithDelayAsync(showDiceResultTime, value);
    }

    private async UniTask ShowDiceResultWithDelayAsync(float delaySeconds, int value, CancellationToken ct = default)
    {
        selectedCard = CardManager.Instance.selectedCard;
        await UniTask.Delay(System.TimeSpan.FromSeconds(delaySeconds), cancellationToken: ct);

        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog("액션!");
        LogManager.Instance.AddLog("");

        if (GameManager.Instance.isPlayerTurn)
        {
            if (selectedCard.C_Type == "Action")
            {
                BattleSystem.ExecuteAttack(PlayerManager.Instance.Player, EnemyManager.Instance.Enemy, selectedCard, value);
                PlayerManager.Instance.PlayAttackAnimation();

                var attackTime = PlayerManager.Instance.Animator.GetCurrentAnimatorStateInfo(0).length;
                await UniTask.Delay(System.TimeSpan.FromSeconds(attackTime * 0.75), cancellationToken: ct);

                EnemyManager.Instance.EnemyHitAnimation();
            }
            else if (selectedCard.C_Type == "Support")
            {
                BattleSystem.ExecuteDefence(PlayerManager.Instance.Player, EnemyManager.Instance.Enemy, selectedCard, value);
                PlayerManager.Instance.PlayAttackAnimation();
            }
            else if (selectedCard.C_Type == "Special")
            {
                BattleSystem.ExecuteSpecial(PlayerManager.Instance.Player, EnemyManager.Instance.Enemy, selectedCard, value);
                PlayerManager.Instance.PlayAttackAnimation();
            }
        }
        else
        {
            if (selectedCard.C_Type == "Action")
            {
                BattleSystem.ExecuteAttack(EnemyManager.Instance.Enemy, PlayerManager.Instance.Player, selectedCard, value);
                EnemyManager.Instance.EnemyAttackAnimation();

                var attackTime = EnemyManager.Instance.Animator.GetCurrentAnimatorStateInfo(0).length;
                await UniTask.Delay(System.TimeSpan.FromSeconds(attackTime * 0.75), cancellationToken: ct);

                PlayerManager.Instance.PlayHitAnimation();
            }
            else if (selectedCard.C_Type == "Support")
            {
                BattleSystem.ExecuteDefence(EnemyManager.Instance.Enemy, PlayerManager.Instance.Player, selectedCard, value);
                EnemyManager.Instance.EnemyAttackAnimation();
            }
            else if (selectedCard.C_Type == "Special")
            {
                BattleSystem.ExecuteSpecial(EnemyManager.Instance.Enemy, PlayerManager.Instance.Player, selectedCard, value);
                EnemyManager.Instance.EnemyAttackAnimation();
            }
        }
        await SwitchTurnWithDelayAsync(switchTurnDelay, ct);
    }

    private async UniTask SwitchTurnWithDelayAsync(float delaySeconds, CancellationToken ct = default)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delaySeconds), cancellationToken: ct);

        if (GameManager.Instance.isPlayerTurn)
        {
            PlayerManager.Instance.Player.OnTurnEnd_WindDecrease();
            PlayerManager.Instance.Player.ProcessEndTurnEffects(PlayerManager.Instance.Player);
        }
        else
        {
            EnemyManager.Instance.Enemy.OnTurnEnd_WindDecrease();
            EnemyManager.Instance.Enemy.ProcessEndTurnEffects(EnemyManager.Instance.Enemy);
        }

        if (PlayerManager.Instance.Player.GetCurrentHp() <= 0 ||
            EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
        {
            return; // 사망 시 턴 전환 안 함
        }
        diceButton.interactable = true;
        GameManager.Instance.SwitchTurn();
    }
}

