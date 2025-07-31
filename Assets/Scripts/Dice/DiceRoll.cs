using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class DiceRoll : MonoBehaviour
{
    public Sprite[] dices; // Inspector���� Dice_1~Dice_6 ������ ����
    private SpriteRenderer _renderer;
    public int DiceResult { get; private set; } // 0~5 �� +1�ϸ� 1~6

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RollDice(System.Action<int> onResult = null)
    {
        Sequence seq = DOTween.Sequence();
        int randomSprite = 0;
        for (int i = 0; i < 60; i++)
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
            onResult?.Invoke(DiceResult + 1); // 1~6 ����
        });
    }
}
