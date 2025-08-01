using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public Enemy Enemy;
    
    [SerializeField] private HPBar hpBar;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float deathShowTime = 2f;
    [SerializeField] public float respawnDelay = 10f;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private GameObject enemyInstance;
    private EnemySO currentEnemySO;
    private Enemy EnemyData;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        _spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        _animator = prefab.GetComponent<Animator>();
        
        hpBar.SetCharacter(Enemy);
    }
    
    public void SetEnemy(int maxHp, Sprite enemySprite, RuntimeAnimatorController enemyAnimatorController) // 적 설정
    {
        Enemy.SetMaxHp(maxHp);
        Enemy.SetCurrentHp(maxHp);
        Enemy.SetHpBar(hpBar);
        hpBar.SetCharacter(Enemy);
        if (_spriteRenderer)
        {
            _spriteRenderer.sprite = enemySprite;
        }

        if (_animator)
        {
            _animator.runtimeAnimatorController = enemyAnimatorController;
        }
    }
    
    public void SpawnEnemy(EnemySO enemy) // 적 소환
    {
        if (enemyInstance)
            Destroy(enemyInstance);

        enemyInstance = Instantiate(prefab, transform.position, transform.rotation);
        enemyInstance.transform.parent = transform;
        Enemy = enemyInstance.GetComponent<Enemy>();
        Enemy.SetEnemySo(enemy);

        
        SetEnemy(enemy.Health, enemy.Sprite, enemy.AnimatorController);
        

        _spriteRenderer = enemyInstance.GetComponent<SpriteRenderer>();
        _animator = enemyInstance.GetComponent<Animator>();
        enemyInstance.SetActive(true);

        currentEnemySO = enemy;
    }

    // Enemy가 사망했을 때 Enemy에서 호출하는 함수
    public void OnEnemyDied()
    {
        //LogManager.Instance.AddLog("적이 사망했습니다.");
        Debug.Log("EnemyManager: 적이 사망했습니다.");

        if (_animator != null)
        {
            _animator.SetBool("enemyIsDie", true);
        }

        // 2초 딜레이 후 오브젝트 파괴 및 초기화
        //StartCoroutine(DestroyEnemyAfterDelay(deathShowTime));
    }

    private IEnumerator DestroyEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemyInstance)
        {
            Destroy(enemyInstance);
            enemyInstance = null;
        }

        Enemy = null;

        if (_spriteRenderer)
            _spriteRenderer.sprite = null;

        StartCoroutine(HandleEnemyDeath());
    }

    private IEnumerator HandleEnemyDeath()
    {
        yield return new WaitForSeconds(deathShowTime);

        if (enemyInstance)
            enemyInstance.SetActive(false);

        Enemy = null;

        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("EnemyManager: 다음 적 소환");

        if (currentEnemySO)
        {
            SpawnEnemy(currentEnemySO);
            GameManager.Instance.EnemyDieTurn();
        }
            
        else
            Debug.LogWarning("EnemyManager: 다음 적 데이터가 없습니다!");
    }

    public void EnemyAttackAnimation()
    {
        _animator.SetTrigger("enemyIsAttack");
    }
    public void EnemyHitAnimation()
    {
        _animator.SetTrigger("enemyIsHit");
    }
}
