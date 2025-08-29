using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card/CardData")]
public class CardDataSO : ScriptableObject
{
    [Header("--- CSV 데이터 ---")]
    public int C_Id;
    public string C_Name;
    public string C_Type;
    public string Element;
    public string Tier;
    public string Rare;
    public string Effect_Type;
    public float min_Value;
    public float Max_Value;
    public float Calculation;
    public string Debuff_Type;
    public int Debuff_Stack;
    public string Buff_Type;
    public int Buff_Stack;
    public string Enhanceable;
    public int Enhance_Count;
    public string Target;
    public string Card_Description;
    public string CardConcept;
}

