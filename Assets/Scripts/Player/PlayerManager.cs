using System;
using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player Player;
    [SerializeField] private HPBar hpBar;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        Player.SetMaxHp(100);
        Player.SetCurrentHp(Player.GetMaxHp());
        Player.SetHpBar(hpBar);
        
        hpBar.SetCharacter(Player);
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
