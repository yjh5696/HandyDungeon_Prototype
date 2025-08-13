using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChoiceSO", menuName = "Scriptable Objects/ChoiceSO")]
public class ChoiceSO : ScriptableObject
{
    [SerializeField] private List<string> choices;
    public List<string> Choices { get => choices; set => choices = value; }
    [SerializeField] private SerializedDictionary<string, string> choiceDescriptions;
    public SerializedDictionary<string, string> ChoiceDescriptions { get => choiceDescriptions; set => choiceDescriptions = value; }
    [SerializeField] private SerializedDictionary<string, string[]> subChoices;
    public SerializedDictionary<string, string[]> SubChoices { get => subChoices; set => subChoices = value; }
    [SerializeField] private SerializedDictionary<string, ChoiceType> choicesTypes;
    public SerializedDictionary<string, ChoiceType> ChoicesTypes { get => choicesTypes; set => choicesTypes = value; }
}
