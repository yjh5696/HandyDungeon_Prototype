using UnityEngine;
using UnityEditor; // 에디터 기능을 사용하기 위해 꼭 필요합니다.
using System.IO;   // 파일 경로(Path)를 다루기 위해 필요합니다.
public class Emeny_CardDataGenerator
{
    // CSV 파일이 위치한 경로를 지정합니다.
    private static readonly string CSV_PATH = "/Resources/M_Card.csv";
    // 생성된 SO 에셋 파일들을 저장할 경로를 지정합니다.
    private static readonly string SO_SAVE_PATH = "Assets/Resources/EnemyCardDataSO/";
    // Unity 에디터 상단 메뉴에 'Tools/Generate CardSOs from CSV' 항목을 추가합니다.
    [MenuItem("Tools/Generate EnemyCardSOs from CSV")]
    public static void GenerateCardSOs()
    {
        // 1. CSV 파일 읽어오기
        string[] allLines = File.ReadAllLines(Application.dataPath + CSV_PATH.Replace("Assets", ""));
        // 저장할 폴더가 없으면 새로 생성합니다.
        if (!Directory.Exists(SO_SAVE_PATH))
        {
            Directory.CreateDirectory(SO_SAVE_PATH);
        }
        // 2. CSV 데이터 한 줄씩 처리하기 (첫 번째 줄은 헤더이므로 건너뜁니다)
        for (int i = 1; i < allLines.Length; i++)
        {
            string line = allLines[i];
            if (string.IsNullOrEmpty(line)) continue;
            string[] fields = line.Split(',');
            // 3. 새 CardDataSO 인스턴스 생성
            CardDataSO cardSO = ScriptableObject.CreateInstance<CardDataSO>();
            // 4. CSV 데이터를 SO 필드에 매핑 (이름이 다른 부분은 최대한 맞춰서 할당)
            // TryParse를 사용하여 데이터가 비어있거나 형식이 맞지 않아도 오류가 나지 않도록 합니다.
            int.TryParse(fields[0], out cardSO.C_Id);
            cardSO.C_Name = fields[1];
            cardSO.C_Type = fields[2];
            cardSO.Element = fields[3];
            cardSO.Effect_Type = fields[4];
            cardSO.Tier = fields[5];
            // E_Coeffici -> Calculation 필드에 매핑
            float.TryParse(fields[7], out cardSO.min_Value);
            float.TryParse(fields[6], out cardSO.Calculation);
            cardSO.Debuff_Type = fields[8];
            int.TryParse(fields[9], out cardSO.Debuff_Stack);
            cardSO.Buff_Type = fields[10];
            int.TryParse(fields[11], out cardSO.Buff_Stack);
            cardSO.Target = fields[12];
            // 마지막 열은 카드의 설명으로 매핑
            if (fields.Length > 13)
            {
                cardSO.Card_Description = fields[13];
            }
            // C_Name이 비어있으면 기본 이름으로 지정
            if (string.IsNullOrEmpty(cardSO.C_Name))
            {
                cardSO.C_Name = $"EnemyCard_{i}";
            }
            // 5. SO 에셋 파일로 저장하기 (파일 이름은 C_Name으로 지정)
            string assetPath = Path.Combine(SO_SAVE_PATH, $"{cardSO.C_Name}.asset");
            AssetDatabase.CreateAsset(cardSO, assetPath);
        }
        // 6. 변경사항 저장 및 에디터 새로고침
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"CardDataSO 생성이 완료되었습니다. {allLines.Length - 1}개의 카드가 생성되었습니다.");
    }
}
