using System;
using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private static readonly int PlayerIsAttack = Animator.StringToHash("playerIsAttack");
    private static readonly int PlayerIsHit = Animator.StringToHash("playerIsHit");
    public Player Player;
    public PlayerAnimator PlayerAnimator;
    private Animator _animator;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private TraitType playerTrait;

    public Animator Animator => _animator;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Player.SetMaxHp(100);
        Player.SetCurrentHp(Player.GetMaxHp());

        Player.SetHpBar(hpBar);
        hpBar.SetCharacter(Player);

        _animator = Player.GetComponent<Animator>();
    }

    public void OnPlayerDied()
    {
        Debug.Log("플레이어 사망!");
        StartCoroutine(GameOverDelayCoroutine());
    }

    private IEnumerator GameOverDelayCoroutine()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.EndBattle(3f).Forget();
    }

    public void PlayAttackAnimation()
    {
        Debug.Log("플레이어 공격 애니메이션");
        _animator.SetTrigger(PlayerIsAttack);
    }

    public void PlayHitAnimation()
    {
        Debug.Log("플레이어 피격 애니메이션");
        _animator.SetTrigger(PlayerIsHit);
    }

    // 플레이어 특성 부여 함수
    public void GrantTraitToPlayer(TraitType trait)
    {
        playerTrait = trait;
    }

    public TraitType GetTraitPlayer()
    {
        return playerTrait;
    }

    public void GrantTraitOne()
    {
        GrantTraitToPlayer(TraitType.Diceby20);
    }
    public void GrantTraitTwo()
    {
        GrantTraitToPlayer(TraitType.Diceby10);
    }
    public void GrantTraitThree()
    {
        GrantTraitToPlayer(TraitType.AddOne);
    }

    public TraitType GetCurrentTrait()
    {
        return playerTrait;
    }
}

