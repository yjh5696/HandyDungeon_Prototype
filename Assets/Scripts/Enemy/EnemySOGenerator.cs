using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EnemySOGenerator : Editor
{
    private static readonly string CSV_PATH = "Assets/Resources/Monster.csv";
    private static readonly string ENEMY_SO_SAVE_PATH = "Assets/Resources/EnemySO/";
    private static readonly string CARD_SO_PATH = "EnemyCardDataSO"; // Resources 폴더 내 경로

    [MenuItem("Tools/Generate EnemySOs from CSV")]
    public static void GenerateEnemySOs()
    {
        TextAsset csvFile = AssetDatabase.LoadAssetAtPath<TextAsset>(CSV_PATH);
        if (csvFile == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {CSV_PATH}");
            return;
        }
        if (!Directory.Exists(ENEMY_SO_SAVE_PATH))
        {
            Directory.CreateDirectory(ENEMY_SO_SAVE_PATH);
        }

        // 모든 카드 SO 미리 로드 (C_Name 키로 딕셔너리 생성)
        CardDataSO[] allCards = Resources.LoadAll<CardDataSO>(CARD_SO_PATH);
        Dictionary<string, CardDataSO> cardNameToSO = new Dictionary<string, CardDataSO>();
        foreach (var card in allCards)
        {
            if (!cardNameToSO.ContainsKey(card.C_Name))
                cardNameToSO.Add(card.C_Name, card);
        }

        string[] records = csvFile.text.Split('\n');
        for (int i = 1; i < records.Length; i++)
        {
            string record = records[i].Trim();
            if (string.IsNullOrEmpty(record)) continue;
            string[] fields = record.Split(',');

            EnemySO enemySO = ScriptableObject.CreateInstance<EnemySO>();

            enemySO.EnemyID = fields[0];
            enemySO.EnemyName = fields[1];
            enemySO.EnemyTribe = fields[2];
            enemySO.EnemyChapter = fields[3];
            enemySO.Element = fields[4];
            enemySO.Rank = fields[5];
            enemySO.Health = float.Parse(fields[6]);

            // 카드 이름 필드 인덱스 (M_Card1=7, M_Card2=8, M_Card3=9, BM_Card=10)
            List<CardDataSO> enemyCards = new List<CardDataSO>();
            for (int col = 7; col <= 10; col++)
            {
                if (col >= fields.Length) break; // 안전 체크
                string cardName = fields[col].Trim();
                if (string.IsNullOrEmpty(cardName) || cardName.ToLower() == "none")
                    continue; // 없는 카드 넘어감

                if (cardNameToSO.TryGetValue(cardName, out CardDataSO cardSO))
                {
                    enemyCards.Add(cardSO);
                }
                else
                {
                    Debug.LogWarning($"'{enemySO.EnemyName}' 몬스터의 카드 '{cardName}'을(를) 찾을 수 없습니다.");
                }
            }
            enemySO.EnemyCards = enemyCards;

            string assetPath = $"{ENEMY_SO_SAVE_PATH}{enemySO.EnemyName}.asset";
            EnemySO existingSO = AssetDatabase.LoadAssetAtPath<EnemySO>(assetPath);
            if (existingSO == null)
            {
                AssetDatabase.CreateAsset(enemySO, assetPath);
            }
            else
            {
                EditorUtility.CopySerialized(enemySO, existingSO);
                EditorUtility.SetDirty(existingSO);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("EnemySO 생성 및 업데이트가 완료되었습니다!");
    }
}
