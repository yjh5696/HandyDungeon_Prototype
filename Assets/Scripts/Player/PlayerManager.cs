using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private int _maxHp = 100;
    private int _currentHp;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _currentHp = _maxHp;
    }
    
    public int GetMaxHp()
    {
        return _maxHp;
    }

    public int GetCurrentHp()
    {
        return _currentHp;
    }
    
    public void SetMaxHp(int maxHp)
    {
        _maxHp = maxHp;
    }

    public void SetCurrentHp(int currentHp)
    {
        _currentHp = currentHp;
    }
}
