using DG.Tweening;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class DiceRoll : MonoBehaviour
{
    public Sprite[] dices;
    private SpriteRenderer _renderer;
    [SerializeField] private int DiceRollTime;
    public int DiceResult { get; private set; }

    private Sequence seq;
    private bool isRolling = false;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void RollDice(Action<int> onResult = null)
    {
        if (isRolling)
        {
            // 이미 굴림 중이면 무시 또는 조치 - 여기선 그냥 리턴
            return;
        }

        isRolling = true;
        seq = DOTween.Sequence();
        int randomSprite = 0;
        int animationFrameCount = DiceRollTime * 20;

        for (int i = 0; i < animationFrameCount; i++)
        {
            seq.AppendCallback(() => {
                randomSprite = Random.Range(0, dices.Length);
                _renderer.sprite = dices[randomSprite];
            });
            seq.AppendInterval(0.05f);
        }
        seq.AppendCallback(() => {
            DiceResult = Random.Range(0, dices.Length);
            _renderer.sprite = dices[DiceResult];
            onResult?.Invoke(DiceResult + 1);
            isRolling = false;
            seq = null;
        });
        seq.Play();
    }

    // 주사위 애니메이션이 돌아가는 중 즉시 결과를 확정하는 함수
    public void ForceFinishRoll(Action<int> onResult)
    {
        if (isRolling && seq != null)
        {
            seq.Kill();  // 애니메이션 강제 종료
            DiceResult = Random.Range(0, dices.Length);
            _renderer.sprite = dices[DiceResult];
            onResult?.Invoke(DiceResult + 1);
            isRolling = false;
            seq = null;
        }
    }
}

