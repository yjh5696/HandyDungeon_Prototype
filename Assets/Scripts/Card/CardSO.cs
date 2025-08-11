using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    [SerializeField] private string cardName;
    public string CardName { get => cardName; set => cardName = value; }
    [SerializeField] private Style style;
    public Style Style { get => style; set => style = value; }
    [SerializeField] private Sprite cardSprite;
    public Sprite CardSprite { get => cardSprite; }
    [SerializeField] private State state;
    public State State { get => state; set => state = value; }
    [SerializeField] private string cardDescription;
    public string CardDescription { get => cardDescription; set => cardDescription = value; }
    [SerializeField] private string stateStrat;
    public string StateStrat { get => stateStrat; set => stateStrat = value; }
    [SerializeField] private float damage;
    public float Damage { get => damage; set => damage = value; }
    [SerializeField] private float diceMultiplier;
    public float DiceMultiplier { get => diceMultiplier; set => diceMultiplier = value; }
    [SerializeField] private ElementType elementType;
    public ElementType ElementType { get => elementType; set => elementType = value; }
}
