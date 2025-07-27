using DG.Tweening;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    public Sprite[] Dices; // Inspector에서 Dice_1~Dice_6 순서로 연결
    private SpriteRenderer renderer;
    public int diceResult { get; private set; } // 0~5 ⇒ +1하면 1~6

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RollDice(System.Action<int> onResult = null)
    {
        Sequence seq = DOTween.Sequence();
        int randomSprite = 0;
        for (int i = 0; i < 15; i++)
        {
            seq.AppendCallback(() => {
                randomSprite = Random.Range(0, Dices.Length);
                renderer.sprite = Dices[randomSprite];
            });
            seq.AppendInterval(0.05f);
        }

        seq.AppendCallback(() => {
            diceResult = Random.Range(0, Dices.Length);
            renderer.sprite = Dices[diceResult];
            onResult?.Invoke(diceResult + 1); // 1~6 전달
        });
    }
}
