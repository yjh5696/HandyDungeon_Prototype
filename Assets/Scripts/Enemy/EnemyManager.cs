using System.Collections;
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
    private GameObject _enemyInstance;
    private EnemySO _currentEnemySo;

    public Animator Animator { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        Animator = prefab.GetComponent<Animator>();

        hpBar.SetCharacter(Enemy);
    }

    public void SetEnemy(float maxHp, Sprite enemySprite, RuntimeAnimatorController enemyAnimatorController)
    {
        Enemy.SetMaxHp(maxHp);
        Enemy.SetCurrentHp(maxHp);
        Enemy.SetHpBar(hpBar);
        hpBar.SetCharacter(Enemy);

        if (_spriteRenderer)
            _spriteRenderer.sprite = enemySprite;

        if (Animator)
            Animator.runtimeAnimatorController = enemyAnimatorController;
    }

    public void SpawnEnemy(EnemySO enemy)
    {
        if (_enemyInstance)
            Destroy(_enemyInstance);

        Debug.Log($"SpawnEnemy 호출: {enemy.EnemyName} 카드 수: {enemy.EnemyCards.Count}");

        _enemyInstance = Instantiate(prefab, transform.position, transform.rotation);
        _enemyInstance.transform.parent = transform;
        Enemy = _enemyInstance.GetComponent<Enemy>();
        Enemy.SetEnemySo(enemy);

        // Enemy에 카드 덱 할당 (EnemySO 내 카드 덱)
        Enemy.SetCards(enemy.EnemyCards);

        SetEnemy(enemy.Health, enemy.Sprite, enemy.AnimatorController);

        _spriteRenderer = _enemyInstance.GetComponent<SpriteRenderer>();
        Animator = _enemyInstance.GetComponent<Animator>();
        _enemyInstance.SetActive(true);
        hpBar.gameObject.SetActive(true);

        _currentEnemySo = enemy;
    }

    public void OnEnemyDied()
    {
        Debug.Log("EnemyManager: 적이 사망했습니다.");
        if (Animator)
            Animator.SetBool("enemyIsDie", true);

        // 죽은 뒤 처리 코루틴 실행
        StartCoroutine(DestroyEnemyAndHpBarAfterDelay(deathShowTime));
    }

    private IEnumerator DestroyEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_enemyInstance)
        {
            Destroy(_enemyInstance);
            _enemyInstance = null;
        }

        Enemy = null;

        if (_spriteRenderer)
            _spriteRenderer.sprite = null;

        StartCoroutine(HandleEnemyDeath());
    }

    private IEnumerator HandleEnemyDeath()
    {
        yield return new WaitForSeconds(deathShowTime);

        if (_enemyInstance)
            _enemyInstance.SetActive(false);

        Enemy = null;

        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("EnemyManager: 다음 적 소환");

        if (_currentEnemySo)
        {
            SpawnEnemy(_currentEnemySo);
            GameManager.Instance.EnemyDieTurn();
        }
        else
        {
            Debug.LogWarning("EnemyManager: 다음 적 데이터가 없습니다!");
        }
    }

    

    private IEnumerator DestroyEnemyAndHpBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_enemyInstance)
        {
            _enemyInstance.SetActive(false);
            //_enemyInstance = null;
        }

        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(false);
            //hpBar = null;
        }

        //Enemy = null;
    }


    public void EnemyAttackAnimation()
    {
        Animator.SetTrigger("enemyIsAttack");
    }

    public void EnemyHitAnimation()
    {
        Animator.SetTrigger("enemyIsHit");
    }
}

