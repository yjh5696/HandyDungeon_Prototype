using TMPro;
using UnityEngine;


public class Card : MonoBehaviour
{
    private CardDataSO _currentCard;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private SpriteRenderer CardImage;

    [SerializeField] private Sprite ActionFire;
    [SerializeField] private Sprite ActionWater;
    [SerializeField] private Sprite ActionLand;
    [SerializeField] private Sprite ActionAir;
    [SerializeField] private Sprite SupportFire;
    [SerializeField] private Sprite SupportWater;
    [SerializeField] private Sprite SupportLand;
    [SerializeField] private Sprite SupportAir;
    [SerializeField] private Sprite Special;


    public void SetCard(CardDataSO card)
    {
        _currentCard = card;
        //spriteRenderer.sprite = card.CardSprite;
        nameText.text = card.C_Name;
        descriptionText.text = card.Card_Description;
        if(card.C_Type == "Special")
            CardImage.sprite = Special;
        else if(card.C_Type == "Support")
            CardImage.sprite = GetSupportCardImage(card.Element);
        else if(card.C_Type == "Action")
            CardImage.sprite = GetActionCardImage(card.Element);
    }

    public Sprite GetActionCardImage(string state)
    {
        switch (state)
        {
            case "Fire": return ActionFire;
            case "Water": return ActionWater;
            case "Air": return ActionAir;
            case "Land": return ActionLand;
            case "None": return null;
            default: return null;
        }
    }

    public Sprite GetSupportCardImage(string state)
    {
        switch (state)
        {
            case "Fire": return SupportFire;
            case "Water": return SupportWater;
            case "Air": return SupportAir;
            case "Land": return SupportLand;
            case "None": return null;
            default: return null;
        }
    }


}
