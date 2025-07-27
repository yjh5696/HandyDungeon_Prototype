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
        Debug.Log($"주사위: {value} / 총 데미지: {totalDamage}");
        // totalDamage를 적이나 플레이어의 체력 등에 적용
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
