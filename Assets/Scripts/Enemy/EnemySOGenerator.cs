// EnemyDataParser.cs
using UnityEngine;
using UnityEditor;
using System.IO;

public class EnemySOGenerator : Editor
{
    // CSV 파일 경로
    private static readonly string CSV_PATH = "Assets/Resources/Monster.csv";

    // 생성된 EnemySO 파일들을 저장할 경로
    private static readonly string ENEMY_SO_SAVE_PATH = "Assets/Resources/EnemySO/";

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

        string[] records = csvFile.text.Split('\n');

        // 헤더(첫 줄)를 건너뛰고 시작
        for (int i = 1; i < records.Length; i++)
        {
            string record = records[i].Trim();
            if (string.IsNullOrEmpty(record)) continue;

            string[] fields = record.Split(',');

            // EnemySO 인스턴스 생성
            EnemySO enemySO = ScriptableObject.CreateInstance<EnemySO>();

            // CSV 데이터 할당 (카드 정보는 제외)
            enemySO.EnemyID = fields[0];
            enemySO.EnemyName = fields[1];
            enemySO.Element = fields[2];
            enemySO.Rank = fields[3];
            enemySO.Health = float.Parse(fields[4]);

            // enemyCards 리스트는 비워둡니다.
            // EnemySO 스크립트에서 new List<CardDataSO>()로 초기화되므로 별도 처리가 필요 없습니다.

            // SO 파일 저장
            string assetPath = $"{ENEMY_SO_SAVE_PATH}{enemySO.EnemyName}.asset";
            // 만약 동일한 이름의 파일이 이미 있다면, 덮어쓰는 대신 기존 데이터를 업데이트합니다.
            EnemySO existingSO = AssetDatabase.LoadAssetAtPath<EnemySO>(assetPath);
            if (existingSO == null)
            {
                AssetDatabase.CreateAsset(enemySO, assetPath);
            }
            else
            {
                EditorUtility.CopySerialized(enemySO, existingSO);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("EnemySO 생성 및 업데이트가 완료되었습니다!");
    }
}