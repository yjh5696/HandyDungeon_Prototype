using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;

public class DiceRoll : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberText;  // 주사위 베이스 위 숫자 텍스트 컴포넌트
    [SerializeField] private int DiceRollTime = 2;   // 굴림 애니메이션 지속 시간(초)

    public int diceFaces = 6;      // 주사위 면 수 (기본 6)
    public int DiceResult { get; private set; }
    private Sequence seq;
    private bool isRolling = false;

    public void RollDice(Action<int> onResult = null)
    {
        if (isRolling) return; // 연속 호출 방지

        isRolling = true;
        seq = DOTween.Sequence();

        int animationFrameCount = DiceRollTime * 20; // 20 프레임/초 가정

        for (int i = 0; i < animationFrameCount; i++)
        {
            seq.AppendCallback(() =>
            {
                int randomNumber = Random.Range(1, diceFaces + 1);
                numberText.text = randomNumber.ToString();
            });

            seq.AppendInterval(0.05f);  // 0.05초 대기 후 다음 숫자 표시
        }

        seq.AppendCallback(() =>
        {
            DiceResult = Random.Range(1, diceFaces + 1);
            numberText.text = DiceResult.ToString();

            onResult?.Invoke(DiceResult);

            isRolling = false;
            seq = null;
        });

        seq.Play();
    }

    // 주사위 애니메이션 중 중단하고 즉시 결과 고정하는 기능
    public void ForceFinishRoll(Action<int> onResult)
    {
        if (isRolling && seq != null)
        {
            seq.Kill();

            DiceResult = Random.Range(1, diceFaces + 1);
            numberText.text = DiceResult.ToString();

            onResult?.Invoke(DiceResult);

            isRolling = false;
            seq = null;
        }
    }
}


