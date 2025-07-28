using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite sprite;
    [SerializeField] private AnimatorController animatorController;
    private int _maxHp;
    private int _currentHp;

    public void SetEnemy(Sprite enemySprite, AnimatorController enemyAnimatorController)
    {
        sprite = enemySprite;
        animatorController = enemyAnimatorController;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
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
