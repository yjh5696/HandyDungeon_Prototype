using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Choice : MonoBehaviour
{
    [SerializeField] private TMP_Text choiceText;
    [SerializeField] private TMP_Text choiceTypeText;
    [SerializeField] private SpriteRenderer choiceTypeSprite;
    [SerializeField] private TMP_Text choiceRateText;
    [SerializeField] private TMP_Text choiceSuccessText;
    [SerializeField] private TMP_Text choiceFailText;
    private ChoiceType _choiceType;
    private ChoiceEvent _subEvent;
    private MainEvent _mainEvent;
    private List<ChoiceEvent> _subChoices;

    public void Init()
    {
        choiceText.text = "";
        choiceTypeText.text = "";
        choiceRateText.text = "";
    }

    public void SetChoice(MainEvent choiceEvent)
    {
        _mainEvent = choiceEvent;
        
        choiceText.text = _mainEvent.choiceName;
        switch (_mainEvent.choiceEventType)
        {
            case ChoiceType.Battle:
                _choiceType = ChoiceType.Battle;
                choiceTypeText.text = "전투";
                choiceTypeSprite.color = Color.red;
                break;
            case ChoiceType.Event:
                _choiceType = ChoiceType.Event;
                choiceTypeText.text = "이벤트";
                choiceTypeSprite.color = Color.blue;
                break;
            case ChoiceType.Treasure:
                _choiceType = ChoiceType.Treasure;
                choiceTypeText.text = "보물";
                choiceTypeSprite.color = Color.yellow;
                break;
            case ChoiceType.Rest:
                _choiceType = ChoiceType.Rest;
                choiceTypeText.text = "휴식";
                choiceTypeSprite.color = Color.green;
                break;
        }
        
        choiceRateText.text = "메인";
        choiceRateText.color = Color.green;
        
        choiceSuccessText.text = "랜덤 카드 1장";
        choiceFailText.text = "";
    }

    public void SetChoice(ChoiceEvent choiceEvent)
    {
        _subEvent = choiceEvent;
        
        choiceText.text = _subEvent.choiceName;
        switch (_subEvent.choiceEventType)
        {
            case ChoiceType.Battle:
                _choiceType = ChoiceType.Battle;
                choiceTypeText.text = "전투";
                choiceTypeSprite.color = Color.red;
                break;
            case ChoiceType.Event:
                _choiceType = ChoiceType.Event;
                choiceTypeText.text = "이벤트";
                choiceTypeSprite.color = Color.blue;
                break;
            case ChoiceType.Treasure:
                _choiceType = ChoiceType.Treasure;
                choiceTypeText.text = "보물";
                choiceTypeSprite.color = Color.yellow;
                break;
            case ChoiceType.Rest:
                _choiceType = ChoiceType.Rest;
                choiceTypeText.text = "휴식";
                choiceTypeSprite.color = Color.green;
                break;
        }
        
        switch (_subEvent.choiceRate)
        {
            case <= 20:
                choiceRateText.text = "낮음";
                choiceRateText.color = Color.red;
                break;
            case <= 50:
                choiceRateText.text = "보통";
                choiceRateText.color = Color.orange;
                break;
            case <= 100:
                choiceRateText.text = "높음";
                choiceRateText.color = Color.green;
                break;
        }

        if (_subEvent.choiceReward != "NONE")
        {
            string[] rewards = _subEvent.choiceReward.Split('/');
            for (int i = 0; i < rewards.Length; i++)
            {
                string[] reward = rewards[i].Split('_');
                switch (reward[0])
                {
                    case "PC":
                        choiceSuccessText.text = $"+카드";
                        break;
                    case "GOLD":
                        choiceSuccessText.text = $"골드 +{reward[1]}";
                        break;
                    case "HP":
                        choiceSuccessText.text = $"체력 +{reward[1]}";
                        break;
                }

                if (i < rewards.Length - 1)
                {
                    choiceSuccessText.text += $", ";
                }
            }
        }

        if (_subEvent.choiceLoss != "NONE")
        {
            string[] losses = _subEvent.choiceLoss.Split('/');
            for (int i = 0; i < losses.Length; i++)
            {
                string[] reward = losses[i].Split('_');
                switch (reward[0])
                {
                    case "PC":
                        choiceFailText.text = $"-카드";
                        break;
                    case "GOLD":
                        choiceFailText.text = $"골드 {reward[1]}";
                        break;
                    case "HP":
                        choiceFailText.text = $"체력 {reward[1]}";
                        break;
                }

                if (i < losses.Length - 1)
                {
                    choiceFailText.text += $", ";
                }
            }
        }
        
    }

    public void ChoiceClicked()
    {
        ChoiceAction().Forget();
    }

    private async UniTaskVoid ChoiceAction()
    {
        switch (Stage.Chapters[GameManager.Instance.currentChapter][GameManager.Instance.currentStage])
        {
            case EventType.MainStory:
                break;
            case EventType.SubStory:
                LogManager.Instance.AddDelayedLog(_subEvent.choiceName, 2.0f).Forget();

                GameManager.Instance.HideChoices();

                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);
                
                LogManager.Instance.StartLog(_subEvent.choiceText).Forget();

                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

                if (_subChoices is { Count: > 0 })
                {
                    GameManager.Instance.ShowChoices();
                    ChoiceManager.Instance.SetSubChoiceButtons(_subChoices);
                }
                else
                {
                    if (_choiceType == ChoiceType.Battle)
                    {
                        GameManager.Instance.StartBattle("Rank1");

                        await UniTask.WaitUntil(() => !GameManager.Instance.isPlayerinBattle); //전투 끝날 때 까지 대기

                        if (GameManager.Instance.lastBattleWon)
                        {
                            LogManager.Instance
                                .StartLog(_subEvent.choiceSuccessText)
                                .Forget();
                        }
                        else
                        {
                            LogManager.Instance
                                .StartLog(_subEvent.choiceFailText).Forget();
                        }
                    }
                    else
                    {
                        int r = Random.Range(1, 101);
                        if (r <= _subEvent.choiceRate)
                        {
                            LogManager.Instance
                                .StartLog(_subEvent.choiceSuccessText)
                                .Forget();
                        }
                        else
                        {
                            LogManager.Instance
                                .StartLog(_subEvent.choiceFailText).Forget();
                        }
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

        await UniTask.WaitForSeconds(3.0f);

        GameManager.Instance.NextStage().Forget();
    }
}