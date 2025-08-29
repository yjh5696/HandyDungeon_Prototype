using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ChoiceSO", menuName = "Scriptable Objects/ChoiceSO")]
public class ChoiceSO : ScriptableObject
{
    [SerializeField] private List<ChoiceEvent> choiceEvent = new List<ChoiceEvent>();
    public List<ChoiceEvent> ChoiceEvent { get => choiceEvent; set => choiceEvent = value; }
    [SerializeField] private SerializedDictionary<string, List<ChoiceEvent>> subChoices = new SerializedDictionary<string, List<ChoiceEvent>>();
    public SerializedDictionary<string, List<ChoiceEvent>> SubChoices { get => subChoices; set => subChoices = value; }
    [SerializeField] private List<MainEvent> mainEvents = new List<MainEvent>();
    public List<MainEvent> MainEvents { get => mainEvents; set => mainEvents = value; }
}
