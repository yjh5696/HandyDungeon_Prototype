#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class CardDataSOGenerator : EditorWindow
{
    private string csvFilePath = "Assets/Resources/GameCard.csv";

    [MenuItem("Tools/CardData SO Generator")]
    public static void ShowWindow()
    {
        GetWindow<CardDataSOGenerator>("CardData SO 생성기");
    }

    void OnGUI()
    {
        GUILayout.Label("CSV 파일 경로 설정", EditorStyles.boldLabel);
        csvFilePath = EditorGUILayout.TextField(csvFilePath);

        if (GUILayout.Button("ScriptableObjects 생성"))
        {
            GenerateCardDataSOs();
        }
    }

    void GenerateCardDataSOs()
    {
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("CSV 파일 경로가 유효하지 않습니다.");
            return;
        }

        string[] lines = File.ReadAllLines(csvFilePath);

        string folderPath = "Assets/Resources/CardDataSOs";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Resources", "CardDataSOs");

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] values = line.Split(',');

            if (values.Length < 24) continue;

            CardDataSO card = ScriptableObject.CreateInstance<CardDataSO>();

            card.C_Id = SafeParseInt(values[0]);
            card.C_Name = values[1];
            card.C_Type = values[2];
            card.Element = values[3];
            card.Tier = values[4];
            card.Rare = values[5];
            card.Effect_Type = values[6];
            card.min_Value = SafeParseFloat(values[7]);
            card.Max_Vlaue = SafeParseFloat(values[8]);
            card.Calculation = SafeParseFloat(values[9]);
            card.Debuff_Type = values[10];
            card.Debuff_Stack = SafeParseInt(values[11]);
            card.Buff_Type = values[12];
            card.Buff_Stack = SafeParseInt(values[13]);
            card.Enhanceable = values[14];
            card.Enhance_Count = string.IsNullOrEmpty(values[15]) ? 0 : SafeParseInt(values[15]);
            card.Target = values[16];
            card.Card_Description = values[17];
            card.Unnamed_18 = values[18];
            card.CardConcept = values[19];
            card.Unnamed_20 = values[20];
            card.Unnamed_21 = values[21];
            card.Unnamed_22 = values[22];
            card.Formula = values[23];

            string assetName = $"{card.C_Name}_{card.C_Id}.asset";
            string savePath = Path.Combine(folderPath, assetName);
            AssetDatabase.CreateAsset(card, savePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("CardData ScriptableObjects 생성 완료!");
    }
    private int SafeParseInt(string s)
    {
        int result;
        if (int.TryParse(s.Trim(), out result))
            return result;
        return 0; // 기본값 또는 -1 등으로 변경 가능
    }

    private float SafeParseFloat(string s)
    {
        float result;
        if (float.TryParse(s.Trim(), out result))
            return result;
        return 0f; // 기본값
    }
}
#endif

