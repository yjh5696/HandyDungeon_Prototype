using System;
using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player Player;

    private void Awake()
    {
        Instance = this;
        Player.SetMaxHp(100);
        Player.SetCurrentHp(Player.GetMaxHp());
    }

    public void OnPlayerDied()
    {
        Debug.Log("Player�� ����Ͽ����ϴ�!");
        StartCoroutine(GameOverDelayCoroutine());
    }
    private IEnumerator GameOverDelayCoroutine()
    {
        yield return new WaitForSeconds(2f); // 2�� ������
        // ���� ���� ���� ����
        Application.Quit();
    }
}
