using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ChoiceSO", menuName = "Scriptable Objects/ChoiceSO")]
public class ChoiceSO : ScriptableObject
{
    [SerializeField] private List<string> choices;
    public List<string> Choices { get => choices; set => choices = value; }
    [SerializeField] private SerializedDictionary<string, int> stageTypes;
    public SerializedDictionary<string, int> StageTypes { get => stageTypes; set => stageTypes = value; }
    [SerializeField] private SerializedDictionary<string, ChoiceType> choicesTypes;
    public SerializedDictionary<string, ChoiceType> ChoicesTypes { get => choicesTypes; set => choicesTypes = value; }
    [SerializeField] private SerializedDictionary<string, string> choiceDescriptions;
    public SerializedDictionary<string, string> ChoiceDescriptions { get => choiceDescriptions; set => choiceDescriptions = value; }
    [SerializeField] private SerializedDictionary<string, string> choiceSucceedDescriptions;
    public SerializedDictionary<string, string> ChoiceSucceedDescriptions { get => choiceSucceedDescriptions; set => choiceSucceedDescriptions = value; }
    [SerializeField] private SerializedDictionary<string, string> choiceFailDescriptions;
    public SerializedDictionary<string, string> ChoiceFailDescriptions { get => choiceFailDescriptions; set => choiceFailDescriptions = value; }
    public string ChoiceImagesPath { get; set; }
    [SerializeField] private SerializedDictionary<string, List<string>> subChoices;
    public SerializedDictionary<string, List<string>> SubChoices { get => subChoices; set => subChoices = value; }
}
