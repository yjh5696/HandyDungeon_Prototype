using Cysharp.Threading.Tasks;
using System;
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
    public bool lastBattleWon = false;
    public bool skipBtnClicked = false;

    public Attack_Button_DiceRoll diceRoll;
    public int currentChapter = 0;
    public int currentStage = 0;
    public int mainStageNumber = 1;
    public StartScriptSO prologueScript;
    public List<StartScriptSO> startScripts = new List<StartScriptSO>();

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
        startFade.FadeOut(1.0f);
        
        StartPrologue().Forget();
        
        //EnemySpawner.Instance.SpawnRandomEnemyByRank("Rank1");
        //StartPlayerTurn();
    }

    public void StartChoice()
    {
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

        foreach (StartScript script in prologueScript.StartScripts)
        {
            LogManager.Instance.AddDelayedLog(script.scriptText, script.delayTime).Forget();
            
            await UniTask.WaitUntil(() => !LogManager.Instance.isLogging || skipBtnClicked);

            if (skipBtnClicked)
            {
                skipBtnClicked = false;
                break;
            }
        }
        
        skip.gameObject.SetActive(false);
 
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

    public void StartScriptLog()
    {
        TraitSet().Forget();
    }

    private async UniTaskVoid TraitSet()
    {
        HideChoices();
        
        LogManager.Instance.PrintScript().Forget();
        
        await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);
        
        choice.gameObject.SetActive(true);
        ChoiceManager.Instance.GetRandomChoice();
    }

    public async UniTaskVoid NextStage()
    {
        currentStage++;
        
        if(Stage.Chapters[currentChapter].Length == currentStage)
        {
            Application.Quit();
            currentStage = 0;
            NextChapter();
            return;
        }
        
        if (Stage.Chapters[currentChapter][currentStage] == EventType.MainStory) //현재 스테이지가 메인이고 스테이지를 진행한다면 다음 메인 선택지가 나오도록 함
        {
            mainStageNumber++;
        }
        
        LogManager.Instance.PrintScript().Forget();
        await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

        switch (Stage.Chapters[currentChapter][currentStage])
        {
            case EventType.Battle:
                LogManager.Instance.ExpandLog();
                LogManager.Instance.isExpandable = false;
                
                LogManager.Instance.AddDelayedLog("적을 만났습니다!", 2.0f).Forget();
            
                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);
            
                LogManager.Instance.AddDelayedLog("전투가 시작됩니다...", 2.0f).Forget();
            
                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

                LogManager.Instance.AddSpacingLine();
                
                LogManager.Instance.isExpandable = true;
                LogManager.Instance.ExpandLog();
            
                StartBattle(Stage.Enemies[currentChapter][currentStage]);
                break;
            case EventType.Boss:
                LogManager.Instance.ExpandLog();
                LogManager.Instance.isExpandable = false;
                
                LogManager.Instance.AddDelayedLog("강력한 적을 만났습니다!", 2.0f).Forget();
            
                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);
            
                LogManager.Instance.AddDelayedLog("전투가 시작됩니다...", 2.0f).Forget();
            
                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);
            
                LogManager.Instance.AddDelayedLog("조심하십시오...", 1.0f).Forget();
            
                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

                LogManager.Instance.AddSpacingLine();
                
                LogManager.Instance.isExpandable = true;
                LogManager.Instance.ExpandLog();
            
                StartBattle(Stage.Enemies[currentChapter][currentStage]);
                break;
            case EventType.MainStory:
            case EventType.SubStory:
                LogManager.Instance.AddSpacingLine();
                StartChoice();
                break;
        }
    }

    public void NextChapter()
    {
        currentChapter++;
    }

    public void StartBattle(string enemyInfo)
    {
        image.FadeOut(1.0f);
        choice.SetActive(false);
        battle.SetActive(true);
        isPlayerinBattle = true;
        EnemySpawner.Instance.SpawnRandomEnemy(enemyInfo);
        StartPlayerTurn();
    }

    public void SwitchTurn()
    {
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
            CardManager.Instance.UpgradeEnhanceableCardsOnTurnEnd();
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

    public async UniTaskVoid EndBattle(float delay)
    {
        await UniTask.WaitForSeconds(delay);
        
        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog("전투 종료");
        LogManager.Instance.AddLog("");

        if (PlayerManager.Instance.Player.GetCurrentHp() <= 0)
        {
            LogManager.Instance.AddLog("플레이어 패배");
            lastBattleWon = false;
            Application.Quit();
            return;
        }
        else if (EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
        {
            LogManager.Instance.AddLog("플레이어 승리");
            lastBattleWon = true;
        }
        
        LogManager.Instance.AddSpacingLine();
        isPlayerinBattle = false;
        
        battle.SetActive(false);
        action.SetActive(false);
        dice.SetActive(false);
        dicebtn.SetActive(true);
        image.gameObject.SetActive(true);
        image.FadeIn(1.0f);
        
        NextStage().Forget();
    }


}

