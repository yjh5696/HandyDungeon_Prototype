using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    [SerializeField] private string cardName;
    public string CardName { get => cardName; }
    [SerializeField] private Style style;
    public Style Style { get => style; }
    [SerializeField] private Sprite cardSprite;
    public Sprite CardSprite { get => cardSprite; }
    [SerializeField] private State state;
    public State State { get => state; }
    [SerializeField] private string cardDescription;
    public string CardDescription { get => cardDescription; }
    [SerializeField] private string stateStrat;
    public string StateStrat { get => stateStrat; }
    [SerializeField] private float damage;
    public float Damage { get => damage; }
    [SerializeField] private float diceMultiplier;
    public float DiceMultiplier { get => diceMultiplier; }

}
