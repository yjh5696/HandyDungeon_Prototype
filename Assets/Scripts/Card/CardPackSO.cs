using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardPackSO", menuName = "Scriptable Objects/CardPackSO")]
public class CardPackSO : ScriptableObject
{
    [SerializeField] private List<CardPack> cardPacks = new List<CardPack>();
    public List<CardPack> CardPacks { get => cardPacks; set => cardPacks = value; }
}
