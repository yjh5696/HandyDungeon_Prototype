using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isPlayerTurn = true;
    public bool isPlayerinBattle = false;
    public bool LastBattleWon = false;
    public bool skipBtnClicked = false;
    public Attack_Button_DiceRoll diceRoll;
    public int currentChapter = 0;
    public int currentStage = 0;
    public int mainStageNumber = 1;

    [SerializeField] private List<EnemySO> enemySOs;
    [SerializeField] private List<CardSO> cardSOs;

    [SerializeField] private FadeInOut startFade;
    [SerializeField] private FadeInOut image;
    [SerializeField] private GameObject log;
    [SerializeField] private GameObject choice;
    [SerializeField] private GameObject battle;
    [SerializeField] private GameObject card;
    [SerializeField] private GameObject action;
    [SerializeField] private GameObject dice;
    [SerializeField] private GameObject dicebtn;
    [SerializeField] private GameObject skip;
    [SerializeField] private GameObject traits;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        startFade.FadeOut(5.0f);
        
        //StartPrologue().Forget();
        
        EnemyManager.Instance.SpawnEnemy(enemySOs[0]);
        StartPlayerTurn();
    }

    public void StartChoice()
    {
        LogManager.Instance.Clear();
        choice.gameObject.SetActive(true);
        ChoiceManager.Instance.GetRandomChoice();
    }

    public void SkipPrologue()
    {
        LogManager.Instance.CancelLog();
        skipBtnClicked = true;
    }

    private async UniTaskVoid StartPrologue()
    {
        skip.GetComponent<Button>().interactable = false;
        await UniTask.WaitUntil(() => !startFade.isFading);
        
        skip.GetComponent<Button>().interactable = true;

        LogManager.Instance.StartLog("{0}프롤로그 시작 중{4.0}스킵해도 됩니다{5.0}이거 봐도 뭐 도움 안 되긴 함 ㅋㅋ{10.0}").Forget();
        
        await UniTask.WaitUntil(() => !LogManager.Instance.isLogging || skipBtnClicked);
        
        skip.gameObject.SetActive(false);
        
        LogManager.Instance.Clear();
        LogManager.Instance.AddLog("특성을 하나 선택하세요.");
        
        traits.SetActive(true);
    }

    public void HideChoices()
    {
        choice.SetActive(false);
    }

    public void ShowChoices()
    {
        choice.SetActive(true);
    }

    public void StartBattle()
    {
        image.FadeOut(1.0f);
        choice.SetActive(false);
        battle.SetActive(true);
        isPlayerinBattle = true;
        EnemyManager.Instance.SpawnEnemy(enemySOs[0]);
        StartPlayerTurn();
    }

    public void SwitchTurn()
    {
        if (isPlayerTurn)
        {
            PlayerManager.Instance.Player.OnTurnEnd_WindDecrease();
            PlayerManager.Instance.Player.ProcessEndTurnEffects();
            // lastElement 유지턴 감소 없음
        }
        else
        {
            EnemyManager.Instance.Enemy.OnTurnEnd_WindDecrease();
            EnemyManager.Instance.Enemy.ProcessEndTurnEffects();
        }

        isPlayerTurn = !isPlayerTurn;

        if (isPlayerTurn)
        {
            log.SetActive(true);
            battle.SetActive(true);
            card.SetActive(false);
            action.SetActive(false);
            dice.SetActive(false);
            dicebtn.SetActive(true);
            StartPlayerTurn();
        }
        else
        {
            StartEnemyTurn();
        }
    }

    public void StartPlayerTurn()
    {
        LogManager.Instance.AddLog("당신의 차례입니다. 액션을 선택해주세요.");
        LogManager.Instance.AddLog($"현재 나의 체력: {PlayerManager.Instance.Player.GetCurrentHp()} / {PlayerManager.Instance.Player.GetMaxHp()}.");
        LogManager.Instance.AddLog($"현재 적의 체력: {EnemyManager.Instance.Enemy.GetCurrentHp()} / {EnemyManager.Instance.Enemy.GetMaxHp()}.");
        CardManager.Instance.DrawCard();
    }

    public void StartEnemyTurn()
    {
        if (!EnemyManager.Instance.Enemy)
        {
            isPlayerTurn = !isPlayerTurn;
            return;
        }

        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog("적의 차례입니다.");
        EnemyManager.Instance.Enemy.DrawAndUseCard();
    }

    public void EnemyDieTurn()
    {
        log.SetActive(true);
        battle.SetActive(true);
        card.SetActive(false);
        action.SetActive(false);
        dice.SetActive(false);
        dicebtn.SetActive(true);
        StartPlayerTurn();
    }

    public void EndGame()
    {
        StartCoroutine(EndBattleAfterDelay(3f));
    }

    private IEnumerator EndBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndBattle();
    }

    private void EndBattle()
    {
        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog("전투 종료");
        LogManager.Instance.AddLog("");

        if (PlayerManager.Instance.Player.GetCurrentHp() <= 0)
        {
            LogManager.Instance.AddLog("플레이어 패배");
            LastBattleWon = false;
        }
        else if (EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
        {
            LogManager.Instance.AddLog("플레이어 승리");
            LastBattleWon = true;
        }
        
        LogManager.Instance.AddSpacingLine();
        isPlayerinBattle = false;
        
        battle.SetActive(false);
        action.SetActive(false);
        dice.SetActive(false);
        dicebtn.SetActive(true);
        image.gameObject.SetActive(true);
        image.FadeIn(1.0f);
    }
}

