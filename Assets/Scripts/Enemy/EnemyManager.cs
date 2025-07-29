using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public Enemy Enemy;
    
    [SerializeField] private GameObject prefab;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private void Awake()
    {
        Instance = this;
        _spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        _animator = prefab.GetComponent<Animator>();
    }

    public void SetEnemy(int maxHp, Sprite enemySprite, AnimatorController enemyAnimatorController) // 적 설정
    {
        Enemy.SetMaxHp(maxHp);
        Enemy.SetCurrentHp(maxHp);
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = enemySprite;
        }

        if (_animator != null)
        {
            _animator.runtimeAnimatorController = enemyAnimatorController;
        }
    }
    
    public void SpawnEnemy(EnemySO enemy) // 적 소환
    {
        var e = Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);
        e.transform.parent = gameObject.transform;

        Enemy = new Enemy();
        Enemy.SetEnemySo(enemy);
        SetEnemy(enemy.Health, enemy.Sprite, enemy.AnimatorController);
    }
}
