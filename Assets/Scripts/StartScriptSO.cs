using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StartScriptsSO", menuName = "Scriptable Objects/StartScriptsSO")]
public class StartScriptSO : ScriptableObject
{
    [SerializeField] private List<StartScript> startScripts = new List<StartScript>();
    public List<StartScript> StartScripts { get => startScripts; set => startScripts = value; }
}
