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
        Player = new Player();
        Player.SetMaxHp(100);
        Player.SetCurrentHp(Player.GetMaxHp());
    }

    public void OnPlayerDied()
    {
        Debug.Log("Player가 사망하였습니다!");
        StartCoroutine(GameOverDelayCoroutine());
    }
    private IEnumerator GameOverDelayCoroutine()
    {
        yield return new WaitForSeconds(2f); // 2초 딜레이
        // 게임 종료 로직 실행
        Application.Quit();
    }
}
