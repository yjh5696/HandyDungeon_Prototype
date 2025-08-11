using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [SerializeField] private TMP_Text choiceText;
    [SerializeField] private TMP_Text choiceTypeText;
    [SerializeField] SpriteRenderer choiceTypeSprite;
    [SerializeField] private TMP_Text choiceDifficultyText;
    [SerializeField] private TMP_Text choiceSuccessText;
    [SerializeField] private TMP_Text choiceFailText;
    private ChoiceType RoomType { get; set; }

    public void SetChoice(string choice, string roomType, string difficulty, string success, string fail)
    {
        choiceText.text = choice;
        switch (roomType)
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
        switch (difficulty)
        {
            case "Low":
                choiceDifficultyText.text = "낮음";
                choiceDifficultyText.color = Color.red;
                break;
            case "Middle":
                choiceDifficultyText.text = "보통";
                choiceDifficultyText.color = Color.orange;
                break;
            case "High":
                choiceDifficultyText.text = "높음";
                choiceDifficultyText.color = Color.green;
                break;
        }
        choiceSuccessText.text = success;
        choiceFailText.text = fail;
    }
}
