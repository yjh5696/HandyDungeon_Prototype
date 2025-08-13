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
    private ChoiceType RoomType { get; set; }
    private List<CardSO> _choiceRewardCard;

    public void SetChoice(string choice, ChoiceType type, int rate)
    {
        choiceText.text = choice;
        switch (choice)
        {
            case "Battle":
                RoomType = ChoiceType.Battle;
                choiceTypeText.text = "전투";
                choiceTypeSprite.color = Color.red;
                break;
            case "Event":
                RoomType = ChoiceType.Event;
                choiceTypeText.text = "이벤트";
                choiceTypeSprite.color = Color.blue;
                break;
            case "Treasure":
                RoomType = ChoiceType.Treasure;
                choiceTypeText.text = "보물";
                choiceTypeSprite.color = Color.yellow;
                break;
            case "Rest":
                RoomType = ChoiceType.Rest;
                choiceTypeText.text = "휴식";
                choiceTypeSprite.color = Color.green;
                break;
        }
        switch (rate)
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

    public void ChoiceClicked()
    {
        if (ChoiceManager.Instance != null)
        {
            
        }
    }
}
