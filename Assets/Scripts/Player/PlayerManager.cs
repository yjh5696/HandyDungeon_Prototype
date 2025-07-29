using System;
using UnityEngine;

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
}
