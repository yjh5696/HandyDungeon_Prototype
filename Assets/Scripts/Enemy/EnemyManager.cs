using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public Enemy Enemy;
    

    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite deathSprite;
    [SerializeField] private float deathShowTime = 2f;
    [SerializeField] private float respawnDelay = 10f;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private GameObject enemyInstance;
    private EnemySO currentEnemySO;
    private Enemy EnemyData;

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
        if (enemyInstance != null)
            Destroy(enemyInstance);

        enemyInstance = Instantiate(prefab, transform.position, transform.rotation);
        enemyInstance.transform.parent = transform;

        Enemy = new Enemy();
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
        LogManager.Instance.AddLog("적이 사망했습니다.");
        Debug.Log("EnemyManager: 적이 사망했습니다.");

        // 사망 스프라이트로 교체
        if (_spriteRenderer != null && deathSprite != null)
            _spriteRenderer.sprite = deathSprite;

        // 애니메이터 중지
        if (_animator != null)
            _animator.runtimeAnimatorController = null;

        // 2초 딜레이 후 오브젝트 파괴 및 초기화
        StartCoroutine(DestroyEnemyAfterDelay(deathShowTime));
    }

    private IEnumerator DestroyEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemyInstance != null)
        {
            Destroy(enemyInstance);
            enemyInstance = null;
        }

        Enemy = null;

        if (_spriteRenderer != null)
            _spriteRenderer.sprite = null;

        StartCoroutine(HandleEnemyDeath());
    }

    private IEnumerator HandleEnemyDeath()
    {
        // 2초 정도 사망 스프라이트 노출 (필요 시 변경)
        yield return new WaitForSeconds(deathShowTime);

        if (enemyInstance != null)
            enemyInstance.SetActive(false);

        Enemy = null;

        // 사망 후 10초 대기
        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("EnemyManager: 다음 적 소환");

        if (currentEnemySO != null)
            SpawnEnemy(currentEnemySO);
        else
            Debug.LogWarning("EnemyManager: 다음 적 데이터가 없습니다!");
    }
}
