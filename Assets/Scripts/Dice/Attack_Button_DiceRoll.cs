using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Attack_Button_DiceRoll : MonoBehaviour
{
    public static Attack_Button_DiceRoll Instance;

    [SerializeField] private DiceRoll leftDiceRoll;    // 플레이어 주사위
    [SerializeField] private DiceRoll rightDiceRoll;   // 적 주사위
    [SerializeField] private GameObject ChanceDice;     // 찬스 주사위 오브젝트

    [SerializeField] private GameObject playerBuffContainer;
    [SerializeField] private GameObject playerDebuffContainer;
    [SerializeField] private GameObject enemyBuffContainer;
    [SerializeField] private GameObject enemyDebuffContainer;

    [SerializeField] private UnityEngine.UI.Button diceButton;

    [SerializeField] private TMP_Text[] playerHistoryTexts;
    [SerializeField] private TMP_Text[] enemyHistoryTexts;
    [SerializeField] private TextMeshProUGUI leftDiceNumberText;

    [SerializeField] private float switchTurnDelay;
    [SerializeField] private float showDiceResultTime;


    private CardDataSO selectedCard = null;

    private CancellationTokenSource cts = null;

    private bool isDiceRolling = false;
    private bool isSkipping = false;
    private bool isProcessing = false;

    private int? enemyDiceValue = null;
    private int? playerDiceValue = null;

    public int playerExtraRollCount = 0;
    public int enemyExtraRollCount = 0;

    private TraitType playerTrait = TraitType.None;

    private int playerHistoryIndex = 0;
    private int enemyHistoryIndex = 0;

    private int playerDiceSum = 0;
    private int enemyDiceSum = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        diceButton.interactable = true;
        TraitType currentTrait = PlayerManager.Instance.GetCurrentTrait();

        // 가져온 특성으로 세팅
        SetPlayerTrait(currentTrait);
    }

    public void SetPlayerTrait(TraitType trait)
    {
        playerTrait = trait;
        switch (playerTrait)
        {
            case TraitType.Diceby20:
                leftDiceRoll.diceFaces = 20;
                rightDiceRoll.diceFaces = 6;
                break;
            case TraitType.Diceby10:
                leftDiceRoll.diceFaces = 10;
                rightDiceRoll.diceFaces = 6;
                break;
            case TraitType.AddOne:
            case TraitType.None:
                leftDiceRoll.diceFaces = 6;
                rightDiceRoll.diceFaces = 6;
                break;
        }
    }

    public void SetPlayerExtraRolls(int count)
    {
        playerExtraRollCount = count;
    }

    public void SetEnemyExtraRolls(int count)
    {
        enemyExtraRollCount = count;
    }

    public void OnAttackButtonClicked()
    {
        Debug.Log("주사위 굴림");
        if (isDiceRolling)
        {
            if (!isSkipping)
            {
                isSkipping = true;
                // 애니메이션 즉시 종료
                leftDiceRoll.ForceFinishRoll(OnLeftDiceRolled);
                rightDiceRoll.ForceFinishRoll(OnRightDiceRolled);
            }
            return;
        }

        isSkipping = false;
        selectedCard = CardManager.Instance.selectedCard;

        isDiceRolling = true;
        enemyDiceValue = null;
        playerDiceValue = null;
        playerDiceSum = 0;
        enemyDiceSum = 0;

        if (selectedCard != null && selectedCard.C_Type == "Special" && GameManager.Instance.isPlayerTurn)
        {
            playerDiceValue = 0; // 플레이어 주사위 무시값
            rightDiceRoll.RollDice(OnRightDiceRolled);
            LogManager.Instance.AddLog("");
            
        }
        else
        {
            leftDiceRoll.RollDice(OnLeftDiceRolled);
            rightDiceRoll.RollDice(OnRightDiceRolled);
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddDelayedLog("주사위를 굴렸습니다!", 1);
        }
    }

    private async void OnLeftDiceRolled(int value)
    {
        diceButton.interactable = false;
        if(playerTrait == TraitType.Diceby20)
        {
            if(value % 2 == 1)
            {
                value = 1;
            }
        }
        else if (playerTrait == TraitType.Diceby10)
        {
            if(value == 2 || value == 3)
            {
                value = 1;
            }
        }
        playerDiceValue = value;
        playerDiceSum += value;

        LogManager.Instance.AddLog("");
        LogManager.Instance.AddDelayedLog($"플레이어의 연기력이 {value}가/이 나왔습니다! (누적합: {playerDiceSum})", 1);
        Debug.Log($"플레이어 주사위 눈이 {value}가/이 나왔습니다! (누적합: {playerDiceSum})");
        TryProcessDiceResult();
    }

    private void OnRightDiceRolled(int value)
    {
        diceButton.interactable = false;
        enemyDiceValue = value;
        enemyDiceSum += value;
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddDelayedLog($"적의 연기력이 {value}가/이 나왔습니다! (누적합: {playerDiceSum})", 1);
        Debug.Log($"적 주사위 눈이 {value}가/이 나왔습니다! (누적합: {enemyDiceSum})");
        TryProcessDiceResult();
    }

    public async void TryProcessDiceResult()
    {
        if (!playerDiceValue.HasValue || !enemyDiceValue.HasValue) return;

        LogManager.Instance.AddDelayedLog($"연기력 합: 플레이어 {playerDiceSum} / 적 {enemyDiceSum}", 1);

        bool playerRollAgain = (playerExtraRollCount > 0);
        bool enemyRollAgain = (enemyExtraRollCount > 0);

        // 딜레이 중 버튼 비활성화, 사용자 입력 제한
        diceButton.interactable = false;

        // 딜레이는 반드시 끝까지 기다림 — 스킵 시 애니만 종료
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        // 딜레이 끝나면 스킵 여부 상관없이 버튼 활성화
        diceButton.interactable = true;
        isSkipping = false; // 딜레이 끝나면 스킵 상태 초기화

        if (playerRollAgain && enemyRollAgain)
        {
            SavePlayerDiceHistory(playerDiceValue.Value);
            SaveEnemyDiceHistory(enemyDiceValue.Value);
            playerExtraRollCount--;
            enemyExtraRollCount--;
            playerDiceValue = null;
            enemyDiceValue = null;

            leftDiceRoll.RollDice(OnLeftDiceRolled);
            rightDiceRoll.RollDice(OnRightDiceRolled);
            LogManager.Instance.AddDelayedLog($"플레이어와 적 모두 추가 주사위 굴림, 남은 횟수: {playerExtraRollCount}, {enemyExtraRollCount}", 1);
            return;
        }
        else if (playerRollAgain)
        {
            SavePlayerDiceHistory(playerDiceValue.Value);
            playerExtraRollCount--;
            playerDiceValue = null;

            leftDiceRoll.RollDice(OnLeftDiceRolled);
            LogManager.Instance.AddDelayedLog($"플레이어 추가 주사위 기회 사용, 남은 횟수: {playerExtraRollCount}", 1);
            return;
        }
        else if (enemyRollAgain)
        {
            SaveEnemyDiceHistory(enemyDiceValue.Value);
            enemyExtraRollCount--;
            enemyDiceValue = null;

            rightDiceRoll.RollDice(OnRightDiceRolled);
            LogManager.Instance.AddDelayedLog($"적 추가 주사위 기회 사용, 남은 횟수: {enemyExtraRollCount}", 1);
            return;
        }

        // 추가 주사위 더 없으면 최종 처리
        isDiceRolling = false;
        diceButton.interactable = false;
        selectedCard = CardManager.Instance.selectedCard;

        if(playerTrait == TraitType.AddOne)
        {
            playerDiceSum += playerDiceSum;
            LogManager.Instance.AddDelayedLog($"플레이어 특성 '1 추가' 적용 후 최종 합: {playerDiceSum}", 1);
        }

        await ShowDiceResultWithDelayAsync(showDiceResultTime, playerDiceSum);
    }


    private void SavePlayerDiceHistory(int value)
    {
        if (playerHistoryIndex < playerHistoryTexts.Length)
        {
            playerHistoryTexts[playerHistoryIndex].text = value.ToString();
            playerHistoryTexts[playerHistoryIndex].gameObject.SetActive(true);
            foreach (Transform child in playerHistoryTexts[playerHistoryIndex].transform)
            {
                child.gameObject.SetActive(true);
            }
            playerHistoryIndex++;
        }
    }

    private void SaveEnemyDiceHistory(int value)
    {
        if (enemyHistoryIndex < enemyHistoryTexts.Length)
        {
            enemyHistoryTexts[enemyHistoryIndex].text = value.ToString();
            enemyHistoryTexts[enemyHistoryIndex].gameObject.SetActive(true);
            foreach (Transform child in enemyHistoryTexts[enemyHistoryIndex].transform)
            {
                child.gameObject.SetActive(true);
            }
            enemyHistoryIndex++;
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
            EnemyManager.Instance.Enemy.ProcessEndTurnEffects(EnemyManager.Instance.Enemy);
            var playerStates = PlayerManager.Instance.Player.GetCurrentStatesWithStacks();
            BuffDebuffUIManager.Instance.UpdateBuffDebuffUI(playerStates, true);  // 플레이어 UI 갱신
        }
        else
        {
            EnemyManager.Instance.Enemy.OnTurnEnd_WindDecrease();
            PlayerManager.Instance.Player.ProcessEndTurnEffects(PlayerManager.Instance.Player);
            EnemyManager.Instance.Enemy.ProcessEndTurnEffects(EnemyManager.Instance.Enemy);
            var enemyStates = EnemyManager.Instance.Enemy.GetCurrentStatesWithStacks();
            BuffDebuffUIManager.Instance.UpdateBuffDebuffUI(enemyStates, false);  // 적 UI 갱신
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
            playerDiceSum = 0;
            enemyDiceSum = 0;
            ChanceDice.SetActive(false);

            // 이전 플레이어 히스토리 초기화
            ClearPlayerDiceHistory();

            // 이전 적 히스토리 초기화
            ClearEnemyDiceHistory();

        }
        else
        {
            diceButton.interactable = false;
        }
    }

    // 히스토리 초기화 함수 정의
    private void ClearPlayerDiceHistory()
    {
        playerHistoryIndex = 0;
        foreach (var text in playerHistoryTexts)
        {
            text.gameObject.SetActive(false);
        }
    }

    private void ClearEnemyDiceHistory()
    {
        enemyHistoryIndex = 0;
        foreach (var text in enemyHistoryTexts)
        {
            text.gameObject.SetActive(false);
        }
    }
}



