using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [SerializeField] private TMP_Text choiceText;
    [SerializeField] private TMP_Text choiceTypeText;
    [SerializeField] private TMP_Text choiceDifficultyText;
    [SerializeField] private TMP_Text choiceSuccessText;
    [SerializeField] private TMP_Text choiceFailText;
    private RoomType _roomType;

    public void SetChoice(string choice, string type, string difficulty, string success, string fail)
    {
        choiceText.text = choice;
        choiceTypeText.text = type;
        choiceDifficultyText.text = difficulty;
        choiceSuccessText.text = success;
        choiceFailText.text = fail;
    }
}
