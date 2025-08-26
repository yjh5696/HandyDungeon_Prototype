using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class toCSV
{
    private static string enemyCSVPath = "/CSV/Enemies.csv";
    private static string cardCSVPath = "/CSV/Cards.csv";
    private static string choiceCSVPath = "/CSV/Choices.csv";
    
    [MenuItem("Utilities/Generate Enemies")]
    public static void GenerateEnemies()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + enemyCSVPath);

        foreach(string allLine in allLines)
        {
            string [] splitData = allLine.Split(',');

            EnemySO enemy = ScriptableObject.CreateInstance<EnemySO>();
            enemy.Name = splitData[0];
            enemy.Health = float.Parse(splitData[1]);
            enemy.Description = splitData[2];
            string [] cards = splitData[3].Split('/');
            foreach(string card in cards)
                enemy.EnemyCards.Add(AssetDatabase.LoadAssetAtPath<CardSO>($"Assets/SO/Cards/{card}.asset"));
            

            AssetDatabase.CreateAsset(enemy, $"Assets/SO/Enemies/{enemy.Name}.asset");
        }

        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Utilities/Generate Cards")]
    public static void GenerateCards()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + cardCSVPath);

        foreach(string allLine in allLines)
        {
            string [] splitData = allLine.Split(',');

            CardSO card = ScriptableObject.CreateInstance<CardSO>();
            card.CardName = splitData[0];
            card.CardDescription = splitData[1];
            switch (splitData[2])
            {
                case "Attack" or "attack":
                    card.Style = Style.Attack;
                    break;
                case "Defense" or "defense":
                    card.Style = Style.Defence;
                    break;
                case "Special" or "special":
                    card.Style = Style.Special;
                    break;
            }
            switch (splitData[3])
            {
                case "Fire" or "fire":
                    card.State = State.Fire;
                    break;
                case "Water" or "water":
                    card.State = State.Water;
                    break;
                case "Wind" or "wind":
                    card.State = State.Wind;
                    break;
                default:
                    card.State = State.None;
                    break;
            }
            card.StateStrat = splitData[4];
            card.Damage = float.Parse(splitData[5]);
            card.DiceMultiplier = float.Parse(splitData[6]);

            AssetDatabase.CreateAsset(card, $"Assets/SO/Cards/{card.CardName}.asset");
        }

        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Utilities/Generate Choices")]
    public static void GenerateChoices()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + choiceCSVPath);

        foreach(string allLine in allLines)
        {
            string [] splitData = allLine.Split(',');

            ChoiceSO choice = ScriptableObject.CreateInstance<ChoiceSO>();
            List<string> descriptions = new List<string>();
            List<string> subChoices = new List<string>();
            ChoiceType choiceType = ChoiceType.Event;
            StageType stageType = StageType.MainStory;
            
            choice.Choices.Add(splitData[0]);
            choice.StageTypes.Add(splitData[0], int.Parse(splitData[1]));
            switch (splitData[2])
            {
                case "Event" or "event":
                    choiceType = ChoiceType.Event;
                    break;
                case "Treasure" or "treasure":
                    choiceType = ChoiceType.Treasure;
                    break;
                case "Battle" or "battle":
                    choiceType = ChoiceType.Battle;
                    break;
                case "Rest" or "rest":
                    choiceType = ChoiceType.Rest;
                    break;
            }
            choice.ChoicesTypes.Add(splitData[0], choiceType);
            choice.ChoiceDescriptions.Add(splitData[0], splitData[3]);
            choice.ChoiceSucceedDescriptions.Add(splitData[0], splitData[4]);
            choice.ChoiceFailDescriptions.Add(splitData[0], splitData[5]);
            choice.ChoiceImagesPath = splitData[6];
            for (int i = 7; i < splitData.Length; i += 3)
            {
                subChoices.Add(splitData[i]);
                choice.ChoiceSucceedDescriptions.Add(splitData[i], splitData[i + 1]);
                choice.ChoiceFailDescriptions.Add(splitData[i], splitData[i + 2]);
            }

            if (subChoices.Count > 0)
            {
                choice.SubChoices.Add(splitData[0], subChoices);
            }

            AssetDatabase.CreateAsset(choice, $"Assets/SO/Choices.asset");
        }

        AssetDatabase.SaveAssets();
    }
}
