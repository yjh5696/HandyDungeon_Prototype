using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Attack_Button_DiceRoll : MonoBehaviour
{
    public static Attack_Button_DiceRoll Instance;
    [SerializeField] private DiceRoll leftDiceRoll;    // 플레이어 주사위
    [SerializeField] private DiceRoll rightDiceRoll;   // 적 주사위
    [SerializeField] private Button diceButton;
    [SerializeField] private float switchTurnDelay;
    [SerializeField] private float showDiceResultTime;

    CardDataSO selectedCard = null;
    private bool isDiceRolling = false;
    private int? enemyDiceValue = null;
    private int? playerDiceValue = null;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        diceButton.interactable = true;
    }

    public void OnAttackButtonClicked()
    {
        selectedCard = CardManager.Instance.selectedCard; // 최신 카드 할당

        if (isDiceRolling)
        {
            // 주사위가 굴러가는 중에 버튼 다시 누르면 모든 주사위 애니메이션 즉시 종료 (스킵)
            leftDiceRoll.ForceFinishRoll(OnLeftDiceRolled);
            rightDiceRoll.ForceFinishRoll(OnRightDiceRolled);
            diceButton.interactable = false; // 중복 스킵 방지용

            return;
        }

        isDiceRolling = true;

        enemyDiceValue = null;
        playerDiceValue = null;

        if (selectedCard != null && selectedCard.C_Type == "Special" && GameManager.Instance.isPlayerTurn)
        {
            // 특수 카드면 플레이어 주사위는 굴리지 않고, 적 주사위만 굴림
            playerDiceValue = 0; // 플레이어 주사위 무시용 값

            rightDiceRoll.RollDice(OnRightDiceRolled);
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddLog("적 주사위를 굴렸습니다!");
        }
        else
        {
            // 일반 카드일 때는 둘 다 굴림
            leftDiceRoll.RollDice(OnLeftDiceRolled);
            rightDiceRoll.RollDice(OnRightDiceRolled);
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddLog("주사위를 굴렸습니다!");
        }
    }

    private async void OnLeftDiceRolled(int value) // 플레이어 주사위 콜백
    {
        playerDiceValue = value;
        diceButton.interactable = false;
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog($"플레이어 주사위 눈이 {value}가/이 나왔습니다!");
        Debug.Log($"플레이어 주사위 눈이 {value}가/이 나왔습니다!");

        TryProcessDiceResult();
    }

    private async void OnRightDiceRolled(int value) // 적 주사위 콜백
    {
        enemyDiceValue = value;
        diceButton.interactable = false;
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog($"적 주사위 눈이 {value}가/이 나왔습니다!");
        Debug.Log($"적 주사위 눈이 {value}가/이 나왔습니다!");
        TryProcessDiceResult();
    }

    public async void TryProcessDiceResult()
    {
        if (enemyDiceValue.HasValue && playerDiceValue.HasValue)
        {
            isDiceRolling = false;
            selectedCard = CardManager.Instance.selectedCard;
            await ShowDiceResultWithDelayAsync(showDiceResultTime, playerDiceValue.Value);
        }
    }

    public async void TryEnemyProCessDiceResult(CardDataSO enemyCard)
    {
        if (isDiceRolling) return;
        isDiceRolling = true;
        selectedCard = enemyCard;
        await ShowDiceResultWithDelayAsync(showDiceResultTime, enemyDiceValue.Value);
    }

    private async UniTask ShowDiceResultWithDelayAsync(float delaySeconds, int value, CancellationToken ct = default)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: ct);

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
                await UniTask.Delay(TimeSpan.FromSeconds(attackTime * 0.75), cancellationToken: ct);

                EnemyManager.Instance.EnemyHitAnimation();
            }
            else if (selectedCard.C_Type == "Support")
            {
                BattleSystem.ExecuteDefence(PlayerManager.Instance.Player, EnemyManager.Instance.Enemy, selectedCard, value);
                PlayerManager.Instance.PlayAttackAnimation();
            }
            else if (selectedCard.C_Type == "Special")
            {
                Debug.Log("플레이어 특수 카드 사용");
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
                await UniTask.Delay(TimeSpan.FromSeconds(attackTime * 0.75), cancellationToken: ct);

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
        await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: ct);

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

        // player, enemy 사망 시 턴 전환 중단
        if (PlayerManager.Instance.Player.GetCurrentHp() <= 0 || EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
        {
            return;
        }

        GameManager.Instance.SwitchTurn();

        if (GameManager.Instance.isPlayerTurn)
        {
            diceButton.interactable = true;
            isDiceRolling = false;
        }
        else
        {
            diceButton.interactable = false;
        }
    }
}

