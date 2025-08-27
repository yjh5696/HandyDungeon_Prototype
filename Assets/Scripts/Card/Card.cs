using TMPro;
using UnityEngine;


public class Card : MonoBehaviour
{
    private CardDataSO _currentCard;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    
    public void SetCard(CardDataSO card)
    {
        _currentCard = card;
        //spriteRenderer.sprite = card.CardSprite;
        nameText.text = card.C_Name;
        descriptionText.text = card.Card_Description;
    }
}
