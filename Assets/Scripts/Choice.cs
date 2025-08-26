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
    [SerializeField] SpriteRenderer choiceTypeSprite;
    [SerializeField] private TMP_Text choiceRateText;
    [SerializeField] private TMP_Text choiceSuccessText;
    [SerializeField] private TMP_Text choiceFailText;
    private string _choice;
    private ChoiceType ChoiceType { get; set; }
    private List<CardSO> _choiceRewardCard;
    private List<string> _subChoices;
    private int _rate;
    private bool _isMainChoice;

    public void Init()
    {
        choiceText.text = "";
        choiceTypeText.text = "";
        choiceRateText.text = "";
    }

    public void SetChoice(string choice, ChoiceType type, int rate, bool isMainStory = false)
    {
        _choice = choice;
        choiceText.text = choice;
        switch (type)
        {
            case ChoiceType.Battle:
                ChoiceType = ChoiceType.Battle;
                choiceTypeText.text = "전투";
                choiceTypeSprite.color = Color.red;
                break;
            case ChoiceType.Event:
                ChoiceType = ChoiceType.Event;
                choiceTypeText.text = "이벤트";
                choiceTypeSprite.color = Color.blue;
                break;
            case ChoiceType.Treasure:
                ChoiceType = ChoiceType.Treasure;
                choiceTypeText.text = "보물";
                choiceTypeSprite.color = Color.yellow;
                break;
            case ChoiceType.Rest:
                ChoiceType = ChoiceType.Rest;
                choiceTypeText.text = "휴식";
                choiceTypeSprite.color = Color.green;
                break;
        }
        
        _rate = rate;
        switch (_rate)
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
    }

    public void SetSubChoices(List<string> subChoices)
    {
        _subChoices = subChoices.ToList();
    }

    public void ChoiceClicked()
    {
        ChoiceAction().Forget();
    }

    private async UniTaskVoid ChoiceAction()
    {
        LogManager.Instance.AddDelayedLog(_choice, 2.0f).Forget();
        
        GameManager.Instance.HideChoices();
        
        await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

        if (ChoiceManager.Instance.choices.ChoiceDescriptions.ContainsKey(_choice))
        {
            LogManager.Instance.StartLog(ChoiceManager.Instance.choices.ChoiceDescriptions[_choice]).Forget();
        }
        
        await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);
        
        if (_subChoices is { Count: > 0 })
        {
            GameManager.Instance.ShowChoices();
            ChoiceManager.Instance.SetSubChoiceButtons(_subChoices);
        }
        else
        {
            if (ChoiceType == ChoiceType.Battle)
            {
                GameManager.Instance.StartBattle();
                
                await UniTask.WaitUntil(() => !GameManager.Instance.isPlayerinBattle); //전투 끝날 때 까지 대기

                if (GameManager.Instance.LastBattleWon)
                {
                    LogManager.Instance.StartLog(ChoiceManager.Instance.choices.ChoiceSucceedDescriptions[_choice]).Forget();
                }
                else
                {
                    LogManager.Instance.StartLog(ChoiceManager.Instance.choices.ChoiceFailDescriptions[_choice]).Forget();
                }
            }
            else
            {
                int r = Random.Range(1, 101);
                if (r <= _rate)
                {
                    LogManager.Instance.StartLog(ChoiceManager.Instance.choices.ChoiceSucceedDescriptions[_choice]).Forget();
                }
                else
                {
                    LogManager.Instance.StartLog(ChoiceManager.Instance.choices.ChoiceFailDescriptions[_choice]).Forget();
                }
            }
            
            await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

            if (Stage.Chapters[GameManager.Instance.currentChapter][GameManager.Instance.currentStage] == StageType.Battle)
            {
                
            }
            else
            {
                GameManager.Instance.ShowChoices();
                ChoiceManager.Instance.GetRandomChoice();
            }
        }
    }
}
