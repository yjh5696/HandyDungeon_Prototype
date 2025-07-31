using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    public static PlayerAnimator Instance { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // ΩÃ±€≈Ê ¡ﬂ∫π πÊ¡ˆ


    }
    public void PlayAttackAnimation()
    {
        Debug.Log("PlayerAttack");
        _animator.SetTrigger("playerIsAttack");
    }
    public void PlayHitAnimation()
    {
        Debug.Log("PlayerHit");
        _animator.SetTrigger("playerIsHit");
    }
}
