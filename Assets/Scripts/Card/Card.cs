using TMPro;
using UnityEngine;


public enum Style
{
    Attack,
    Defence,
    Special
}

public enum State
{
    None = 0,
    Fire = 1,
    Water = 2,
    Wind= 3
}

public class Card : MonoBehaviour
{
    private CardSO _currentCard;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    public void SetCard(CardSO card)
    {
        _currentCard = card;
        spriteRenderer.sprite = card.CardSprite;
        nameText.text = card.CardName;
        descriptionText.text = card.CardDescription;
    }
}
