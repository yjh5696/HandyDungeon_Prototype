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

            if(splitData.Length != 4)
            {
                Debug.Log(allLine + " Does not have 4 values");
            }

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

            if(splitData.Length != 7)
            {
                Debug.Log(allLine + " Does not have 7 values");
            }

            CardSO card = ScriptableObject.CreateInstance<CardSO>();
            card.CardName = splitData[0];
            card.CardDescription = splitData[1];
            switch (splitData[2])
            {
                case "Attack":
                    card.Style = Style.Attack;
                    break;
                case "Defense":
                    card.Style = Style.Defence;
                    break;
                case "Special":
                    card.Style = Style.Special;
                    break;
            }
            switch (splitData[3])
            {
                case "Fire":
                    card.State = State.Fire;
                    break;
                case "Water":
                    card.State = State.Water;
                    break;
                case "Wind":
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

            if(splitData.Length != 5)
            {
                Debug.Log(allLine + " Does not have 5 values");
            }

            ChoiceSO choice = ScriptableObject.CreateInstance<ChoiceSO>();
            choice.ChoiceName = splitData[0];
            choice.ChoiceType = splitData[1];
            choice.ChoiceDifficulty = splitData[2];
            choice.ChoiceSuccess = splitData[3];
            choice.ChoiceFail = splitData[4];
            

            AssetDatabase.CreateAsset(choice, $"Assets/SO/Choices/{choice.ChoiceName}.asset");
        }

        AssetDatabase.SaveAssets();
    }
}
