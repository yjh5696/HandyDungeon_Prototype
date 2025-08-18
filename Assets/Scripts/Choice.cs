using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [SerializeField] private TMP_Text choiceText;
    [SerializeField] private TMP_Text choiceTypeText;
    [SerializeField] SpriteRenderer choiceTypeSprite;
    [SerializeField] private TMP_Text choiceRateText;
    [SerializeField] private TMP_Text choiceSuccessText;
    [SerializeField] private TMP_Text choiceFailText;
    private ChoiceType ChoiceType { get; set; }
    private List<CardSO> _choiceRewardCard;
    private string[] _subChoices;
    private int _rate;

    public void Init()
    {
        choiceText.text = "";
        choiceTypeText.text = "";
        choiceRateText.text = "";
        _subChoices = Array.Empty<string>();
    }

    public void SetChoice(string choice, ChoiceType type, int rate)
    {
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

    public void SetSubChoices(string[] subChoices)
    {
        _subChoices = subChoices;
    }

    public void ChoiceClicked()
    {
        if (_subChoices is { Length: > 0 })
        {
            ChoiceManager.Instance.SetSubChoiceButtons(_subChoices);
        }
        else
        {
            
        }
    }
}
