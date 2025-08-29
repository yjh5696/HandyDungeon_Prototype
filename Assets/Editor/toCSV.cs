using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class toCSV
{
    private static string mainCSVPath = "/Resources/CSV/Choice/Main.csv";
    private static string subCSVPath = "/Resources/CSV/Choice/Sub.csv";
    private static string scriptCSVPath = "/Resources/CSV/StartScripts/";
    
    [MenuItem("Utilities/Generate Sub Choices")]
    public static void GenerateChoices()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + subCSVPath);
        ChoiceSO choices = ScriptableObject.CreateInstance<ChoiceSO>();

        foreach(string allLine in allLines)
        {
            string [] splitData = allLine.Split(',');
            ChoiceEvent choiceEvent = new ChoiceEvent();
            
            choiceEvent.choiceID = splitData[0];
            choiceEvent.choiceChapterNumber = int.Parse(splitData[1]);
            choiceEvent.choiceEventNumber = int.Parse(splitData[2]);
            choiceEvent.isRootEvent = int.Parse(splitData[3]) != 0;
            choiceEvent.parentChoiceID = splitData[4];
            choiceEvent.choiceEventType = splitData[5] switch
            {
                "EVENT" => ChoiceType.Event,
                "TREASURE" => ChoiceType.Treasure,
                "BATTLE" => ChoiceType.Battle,
                "REST" => ChoiceType.Rest,
                _ => choiceEvent.choiceEventType
            };
            choiceEvent.choiceName = splitData[6];
            choiceEvent.choiceText = splitData[7];
            choiceEvent.choiceRate = float.Parse(splitData[8]);
            choiceEvent.choiceSuccessText = splitData[9];
            choiceEvent.choiceFailText = splitData[10];
            choiceEvent.choiceReward = splitData[11];
            choiceEvent.choiceLoss = splitData[12];
            choiceEvent.choiceRequirement = splitData[13];
            
            choices.ChoiceEvent.Add(choiceEvent);
        }

        foreach (ChoiceEvent ev in choices.ChoiceEvent.Where(ev => ev.parentChoiceID != "NONE"))
        {
            if (!choices.SubChoices.ContainsKey(ev.parentChoiceID))
            {
                List<ChoiceEvent> events = new List<ChoiceEvent>();
                choices.SubChoices.Add(ev.parentChoiceID, events);
            }
            choices.SubChoices[ev.parentChoiceID].Add(ev);
        }
        
        AssetDatabase.CreateAsset(choices, $"Assets/Resources/Sub.asset");
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Utilities/Generate Main Choices")]
    public static void GenerateMainChoices()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + mainCSVPath);
        ChoiceSO choices = ScriptableObject.CreateInstance<ChoiceSO>();

        foreach(string allLine in allLines)
        {
            string [] splitData = allLine.Split(',');
            MainEvent mainEvent = new MainEvent();
            
            mainEvent.choiceID = splitData[0];
            mainEvent.choiceStageNumber = int.Parse(splitData[1]);
            mainEvent.choiceEventNumber = int.Parse(splitData[2]);
            mainEvent.choiceEventType = splitData[3] switch
            {
                "EVENT" => ChoiceType.Event,
                "TREASURE" => ChoiceType.Treasure,
                "BATTLE" => ChoiceType.Battle,
                "REST" => ChoiceType.Rest,
                _ => mainEvent.choiceEventType
            };
            mainEvent.choiceName = splitData[4];
            mainEvent.choiceText = splitData[5];
            mainEvent.choiceRate = float.Parse(splitData[6]);
            mainEvent.choiceSuccessText = splitData[7];
            mainEvent.choiceFailText = splitData[8];
            mainEvent.choiceReward = splitData[9];
            mainEvent.choiceLoss = splitData[10];
            
            choices.MainEvents.Add(mainEvent);
        }
        
        AssetDatabase.CreateAsset(choices, $"Assets/Resources/Main.asset");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Start Script")]
    public static void GenerateStartScript()
    {
        string[] filesPath = Directory.GetFiles(Application.dataPath + scriptCSVPath, "*.csv");
        
        foreach (string path in filesPath)
        {
            StartScriptSO startScriptSO = ScriptableObject.CreateInstance<StartScriptSO>();
            string[] allLines = File.ReadAllLines(path);
            
            foreach (string allLine in allLines)
            {
                string [] splitData = allLine.Split(',');
                StartScript startScript = new StartScript();
                
                startScript.scriptID = splitData[0];
                startScript.chapterID = int.Parse(splitData[1]);
                startScript.eventID = int.Parse(splitData[2]);
                startScript.scriptText = splitData[4];
                startScript.delayTime = float.Parse(splitData[5]);
                
                startScriptSO.StartScripts.Add(startScript);
            }
            
            AssetDatabase.CreateAsset(startScriptSO, $"Assets/Resources/StartScripts/{Path.GetFileNameWithoutExtension(path)}.asset");
            AssetDatabase.SaveAssets();
        }
    }
}
