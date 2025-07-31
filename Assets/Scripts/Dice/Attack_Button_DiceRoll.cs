using UnityEngine;
using UnityEngine.UIElements;
using static Attack_Button_DiceRoll;
using static Enemy;
using static UnityEngine.Rendering.DebugUI;

public class Attack_Button_DiceRoll : MonoBehaviour
{
    public static Attack_Button_DiceRoll Instance;
    private Animator _animator;
    public DiceRoll diceRoll;
    [SerializeField] private float switchTurnDelay;
    [SerializeField] private float showDiceResultTime;
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
        //CardSO selectedCard = CardManager.Instance.selectedCard;
        //Enemy currentEnemy = EnemyManager.Instance.Enemy;
        //float totalDamage = CardManager.Instance.selectedCard.Damage * (value * CardManager.Instance.selectedCard.DiceMultiplier);
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog($"주사위 눈이 {value}가/이 나왔습니다!");
        Debug.Log($"주사위 눈이 {value}가/이 나왔습니다!");

        StartCoroutine(ShowDiceResultWithDelay(showDiceResultTime, value));
    }

    private System.Collections.IEnumerator SwitchTurnWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        if(PlayerManager.Instance.Player.GetCurrentHp() <= 0)
        {
            //PlayerManager.Instance.Player.PlayerDie();
            yield break; // 플레이어가 사망한 경우 더 이상 진행하지 않음
        }
        else if (EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
        {
            //EnemyManager.Instance.Enemy.EnemyDie();
            yield break; // 적이 사망한 경우 더 이상 진행하지 않음
        }
        else {
            GameManager.Instance.SwitchTurn();
        }
            
    }
    
    private System.Collections.IEnumerator ShowDiceResultWithDelay(float delaySeconds, int value)
    {
        yield return new WaitForSeconds(delaySeconds);
        CardSO selectedCard = CardManager.Instance.selectedCard;
        Enemy currentEnemy = EnemyManager.Instance.Enemy;
        float totalDamage = CardManager.Instance.selectedCard.Damage * (value * CardManager.Instance.selectedCard.DiceMultiplier);

        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog("액션!");
        LogManager.Instance.AddLog("");
        
        if (GameManager.Instance.isPlayerTurn)
        {
            PlayerManager.Instance.PlayAttackAnimation();
            EnemyManager.Instance.EnemyHitAnimation();
            //LogManager.Instance.AddLog($"{EnemyManager.Instance.Enemy.GetEnemySo().Name}에게 {totalDamage}의 데미지를 주었습니다!"); 
            //EnemyManager.Instance.Enemy.SetCurrentHp(EnemyManager.Instance.Enemy.GetCurrentHp() - totalDamage);
            State enemyCardState = selectedCard.State;
            EnemyManager.Instance.Enemy.EnemyTakeDamage(totalDamage, (Enemy.EnemyStatusEffect)enemyCardState);
            currentEnemy.EnemyApplyStatus((Enemy.EnemyStatusEffect)enemyCardState);
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddSpacingLine();
        }
        else
        {
            EnemyManager.Instance.EnemyAttackAnimation();
            PlayerManager.Instance.PlayHitAnimation();
            //LogManager.Instance.AddLog($"플레이어에게 {totalDamage}의 데미지를 주었습니다!");
            //PlayerManager.Instance.Player.SetCurrentHp(PlayerManager.Instance.Player.GetCurrentHp() - totalDamage);
            State playerCardState = selectedCard.State;
            PlayerManager.Instance.Player.PlayerTakeDamage(totalDamage, (Player.PlayerStatusEffect)playerCardState);
            PlayerManager.Instance.Player.PlayerApplyStatus((Player.PlayerStatusEffect)playerCardState);
            LogManager.Instance.AddLog("");
            LogManager.Instance.AddSpacingLine();
        }
        StartCoroutine(SwitchTurnWithDelay(switchTurnDelay));
    }
}
