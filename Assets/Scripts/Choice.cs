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
    private string[] _subChoices = null;
    private int _rate;

    public void SetChoice(string choice, ChoiceType type)
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

        int[] rates = { 20, 50, 70 };
        int r = Random.Range(0, rates.Length);
        _rate = rates[r];
        switch (r)
        {
            case 0:
                choiceRateText.text = "낮음";
                choiceRateText.color = Color.red;
                break;
            case 1:
                choiceRateText.text = "보통";
                choiceRateText.color = Color.orange;
                break;
            case 2:
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
        if (ChoiceManager.Instance != null)
        {
            
        }
    }
}
