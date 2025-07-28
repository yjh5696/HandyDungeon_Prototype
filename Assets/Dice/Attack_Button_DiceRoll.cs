using UnityEngine;

public class Attack_Button_DiceRoll : MonoBehaviour
{
    public DiceRoll diceRoll;
    public int baseValue = 5;

    public void OnAttackButtonClicked()
    {
        diceRoll.RollDice(OnDiceRolled);
    }
    void OnDiceRolled(int value)
    {
        int totalDamage = baseValue * value;
        Debug.Log($"�ֻ���: {value} / �� ������: {totalDamage}");
        // totalDamage�� ���̳� �÷��̾��� ü�� � ����
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
