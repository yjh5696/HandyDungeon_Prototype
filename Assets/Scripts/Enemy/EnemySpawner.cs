// EnemySpawner.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // C#의 강력한 데이터 처리 기능인 LINQ를 사용하기 위해 꼭 필요합니다!

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    // Resources 폴더에서 불러온 모든 EnemySO 원본 데이터를 담아둘 리스트
    private List<EnemySO> enemyDatabase = new List<EnemySO>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadAllEnemies();
    }

    private void LoadAllEnemies()
    {
        enemyDatabase = Resources.LoadAll<EnemySO>("EnemySO").ToList();

        if (enemyDatabase.Count == 0)
        {
            Debug.LogError("Resources/EnemySO 폴더에 불러올 EnemySO가 없습니다!");
        }
    }

    // --- 이 부분이 핵심입니다! ---

    /// <summary>
    /// 지정된 Rank를 가진 적들 중에서만 랜덤으로 하나를 골라 소환합니다.
    /// </summary>
    /// <param name="rank">소환하고 싶은 적의 Rank (예: "Rank1", "Rank2")</param>
    public void SpawnRandomEnemyByRank(string rank)
    {
        // 1. 전체 데이터베이스에서 Rank가 일치하는 적들만 골라내 새로운 리스트를 만듭니다.
        List<EnemySO> filteredList = enemyDatabase.Where(enemy => enemy.Rank == rank).ToList();

        // 2. 골라낸 적이 있는지 확인합니다.
        if (filteredList.Count == 0)
        {
            Debug.LogWarning($"'{rank}' Rank를 가진 적을 찾을 수 없습니다.");
            return;
        }

        // 3. 골라낸 리스트 안에서 랜덤으로 한 명을 선택합니다.
        int randomIndex = Random.Range(0, filteredList.Count);
        EnemySO enemyToSpawn = filteredList[randomIndex];

        // 4. EnemyManager에게 선택된 적을 소환하라고 명령합니다.
        EnemyManager.Instance.SpawnEnemy(enemyToSpawn);
    }

    public void SpawnRandomEnemy(string info)
    {
        string[] splitInfo = info.Split('_');
        
        List<EnemySO> filteredList = enemyDatabase.Where(enemy => enemy.Rank == splitInfo[0] && enemy.EnemyTribe == splitInfo[1]).ToList();
        
        if (filteredList.Count == 0)
        {
            Debug.LogWarning($"'{splitInfo[0]}' Rank / '{splitInfo[1]}' TribeType를 가진 적을 찾을 수 없습니다.");
            return;
        }
        
        int randomIndex = Random.Range(0, filteredList.Count);
        EnemySO enemyToSpawn = filteredList[randomIndex];
        
        EnemyManager.Instance.SpawnEnemy(enemyToSpawn);
    }

    /// <summary>
    /// (참고용) 모든 적 중에서 무작위로 하나를 소환하는 기존 함수
    /// </summary>
    public void SpawnRandomEnemy()
    {
        if (enemyDatabase.Count == 0)
        {
            Debug.LogWarning("소환할 적 데이터가 없습니다.");
            return;
        }

        int randomIndex = Random.Range(0, enemyDatabase.Count);
        EnemySO randomEnemyToSpawn = enemyDatabase[randomIndex];
        EnemyManager.Instance.SpawnEnemy(randomEnemyToSpawn);
    }
}