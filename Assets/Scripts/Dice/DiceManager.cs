using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    public DiceRoll playerDicePrefab;
    public DiceRoll enemyDicePrefab;

    public Transform[] playerDicePositions;  // 주사위 위치 배열 (최소 1 이상)
    public Transform[] enemyDicePositions;

    public float diceMoveTime = 0.5f;

    private List<DiceRoll> playerDices = new List<DiceRoll>();
    private List<DiceRoll> enemyDices = new List<DiceRoll>();

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnPlayerDiceButton()
    {
        SpawnPlayerDice();
    }

    public void SpawnEnemyDiceButton()
    {
        SpawnEnemyDice();
    }

    // 플레이어 기본 주사위 1개 생성 (중복 방지)
    public DiceRoll SpawnPlayerDice()
    {
        if (playerDices.Count > 0)
            return playerDices[0];
        var dice = Instantiate(playerDicePrefab, playerDicePositions[0].position, Quaternion.identity, playerDicePositions[0].transform);
        playerDices.Add(dice);
        return dice;
    }

    public DiceRoll SpawnEnemyDice()
    {
        if (enemyDices.Count > 0)
            return enemyDices[0];
        var dice = Instantiate(enemyDicePrefab, enemyDicePositions[0].position, Quaternion.identity, enemyDicePositions[0].transform);
        enemyDices.Add(dice);
        return dice;
    }

    public void SpawnExtraDice(int playerCount, int enemyCount)
    {
        if (playerCount > 0)
        {
            for (int i = 0; i < playerDices.Count; i++)
            {
                Vector3 targetPos = playerDicePositions[i + playerCount].position;
                playerDices[i].transform.DOMove(targetPos, diceMoveTime);
                playerDices[i].transform.DOScale(Vector3.one * 0.5f, diceMoveTime);
            }
            for (int i = 0; i < playerCount; i++)
            {
                var newDice = Instantiate(playerDicePrefab, playerDicePositions[i].position, Quaternion.identity, transform);
                newDice.transform.localScale = Vector3.zero;
                newDice.transform.DOScale(Vector3.one * 0.5f, diceMoveTime);
                playerDices.Insert(i, newDice);
            }
        }

        if (enemyCount > 0)
        {
            for (int i = 0; i < enemyDices.Count; i++)
            {
                Vector3 targetPos = enemyDicePositions[i + enemyCount].position;
                enemyDices[i].transform.DOMove(targetPos, diceMoveTime);
                enemyDices[i].transform.DOScale(Vector3.one * 0.5f, diceMoveTime);
            }
            for (int i = 0; i < enemyCount; i++)
            {
                var newDice = Instantiate(enemyDicePrefab, enemyDicePositions[i].position, Quaternion.identity, transform);
                newDice.transform.localScale = Vector3.zero;
                newDice.transform.DOScale(Vector3.one * 0.5f, diceMoveTime);
                enemyDices.Insert(i, newDice);
            }
        }
    }

    // 플레이어 주사위 값 총합
    public int GetTotalPlayerDiceValue()
    {
        int total = 0;
        foreach (var dice in playerDices)
        {
            total += dice.DiceResult;
        }
        return total;
    }

    // 적 주사위 값 총합
    public int GetTotalEnemyDiceValue()
    {
        int total = 0;
        foreach (var dice in enemyDices)
        {
            total += dice.DiceResult;
        }
        return total;
    }
}

