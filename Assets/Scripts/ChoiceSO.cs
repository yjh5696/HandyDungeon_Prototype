using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ChoiceSO", menuName = "Scriptable Objects/ChoiceSO")]
public class ChoiceSO : ScriptableObject
{
    [SerializeField] private string choiceName;
    public string ChoiceName { get => choiceName; set => choiceName = value; }

    [SerializeField] private string choiceType;
    public string ChoiceType { get => choiceType; set => choiceType = value; }

    [SerializeField] private string choiceDifficulty;
    public string ChoiceDifficulty { get => choiceDifficulty; set => choiceDifficulty = value; }

    [SerializeField] private string choiceSuccess;
    public string ChoiceSuccess { get => choiceSuccess; set => choiceSuccess = value; }

    [SerializeField] private string choiceFail;
    public string ChoiceFail { get => choiceFail; set => choiceFail = value; }
}
