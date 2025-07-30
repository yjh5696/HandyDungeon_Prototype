using UnityEngine;
using UnityEngine.UIElements;
using static Attack_Button_DiceRoll;
using static Enemy;

public class Attack_Button_DiceRoll : MonoBehaviour
{
    public DiceRoll diceRoll;
    public int baseValue = 5;
    public bool playing = false;
    public enum AttackType { Fire, Water, Wind }
    

    public void OnAttackButtonClicked()
    {
        diceRoll.RollDice(OnDiceRolled);
        if (GameManager.Instance.isPlayerTurn)
        {
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddLog("주사위를 굴렸습니다!");
        }
        else
        {
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddLog($"{EnemyManager.Instance.Enemy.GetEnemySo().Name}이/가 주사위를 굴렸습니다!");
        }
    }
    void OnDiceRolled(int value)
    {
        CardSO selectedCard = CardManager.Instance.selectedCard;
        Enemy currentEnemy = EnemyManager.Instance.Enemy;
        float totalDamage = CardManager.Instance.selectedCard.Damage * (value * CardManager.Instance.selectedCard.DiceMultiplier);
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog($"주사위 눈이 {value}가/이 나왔습니다!");
        Debug.Log($"주사위 눈이 {value}가/이 나왔습니다!");
        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog("액션!");
        LogManager.Instance.AddLog("");
        if (GameManager.Instance.isPlayerTurn)
        {
            LogManager.Instance.AddLog($"{EnemyManager.Instance.Enemy.GetEnemySo().Name}에게 {totalDamage}의 데미지를 주었습니다!");
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddSpacingLine();
            //EnemyManager.Instance.Enemy.SetCurrentHp(EnemyManager.Instance.Enemy.GetCurrentHp() - totalDamage);
            State enemyCardState = selectedCard.State + 1;
            EnemyManager.Instance.Enemy.EnemyTakeDamage(totalDamage, (Enemy.EnemyStatusEffect)enemyCardState);
            currentEnemy.EnemyApplyStatus((Enemy.EnemyStatusEffect)enemyCardState);
        }
        else
        {
            LogManager.Instance.AddLog($"플레이어에게 {totalDamage}의 데미지를 주었습니다!");
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddSpacingLine();
            //PlayerManager.Instance.Player.SetCurrentHp(PlayerManager.Instance.Player.GetCurrentHp() - totalDamage);
            State playerCardState = selectedCard.State + 1;
            PlayerManager.Instance.Player.PlayerTakeDamage(totalDamage, (Player.PlayerStatusEffect)playerCardState);
            PlayerManager.Instance.Player.PlayerApplyStatus((Player.PlayerStatusEffect)playerCardState);
        }
        StartCoroutine(SwitchTurnWithDelay(3f));
    }

    private System.Collections.IEnumerator SwitchTurnWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        GameManager.Instance.SwitchTurn();
    }
}
