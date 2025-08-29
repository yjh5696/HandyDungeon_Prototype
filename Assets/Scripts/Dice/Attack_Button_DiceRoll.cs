using UnityEngine;
using UnityEngine.UIElements;
using static Attack_Button_DiceRoll;
using static Enemy;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;

public class Attack_Button_DiceRoll : MonoBehaviour
{
    public static Attack_Button_DiceRoll Instance;
    public DiceRoll diceRoll;

    [SerializeField] private float switchTurnDelay;
    [SerializeField] private float showDiceResultTime;
    CardDataSO selectedCard = null;
    CardDataSO SpecialCard = null;
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        SpecialCard = CardManager.Instance.selectedCard;
        if (SpecialCard != null && SpecialCard.C_Type == "Special" && GameManager.Instance.isPlayerTurn)
        {
            int fakeDiceValue = 0;
            StartCoroutine(ShowDiceResultWithDelay(0, fakeDiceValue));
        }
    }

    public void OnAttackButtonClicked()
    {

        if(SpecialCard.C_Type == "Special" && GameManager.Instance.isPlayerTurn)
        {
            Debug.Log("플레이어 특수 카드 사용 - 주사위 굴리지 않음");
            return;
        }
        else
        {
            // 주사위 굴리기
            diceRoll.RollDice(OnDiceRolled);

            // 로그 출력
            LogManager.Instance.AddLog("");
            if (GameManager.Instance.isPlayerTurn)
            {
                LogManager.Instance.AddLog("주사위를 굴렸습니다!");
            }
            else
            {
                string enemyName = EnemyManager.Instance.Enemy.GetEnemySo().EnemyName;
                LogManager.Instance.AddLog($"{enemyName}이/가 주사위를 굴렸습니다!");
            }
        }
    }

    void OnDiceRolled(int value)
    {
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog($"주사위 눈이 {value}가/이 나왔습니다!");
        Debug.Log($"주사위 눈이 {value}가/이 나왔습니다!");

        StartCoroutine(ShowDiceResultWithDelay(showDiceResultTime, value));
    }

    private IEnumerator ShowDiceResultWithDelay(float delaySeconds, int value)
    {
        selectedCard = CardManager.Instance.selectedCard;
        yield return new WaitForSeconds(delaySeconds);

        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog("액션!");
        LogManager.Instance.AddLog("");

        

        if (GameManager.Instance.isPlayerTurn)
        {
            // 플레이어 공격 → 적 피격
            if (selectedCard.C_Type == "Action")
            {
                BattleSystem.ExecuteAttack(PlayerManager.Instance.Player, EnemyManager.Instance.Enemy, selectedCard, value);
                PlayerManager.Instance.PlayAttackAnimation();
                float attackTime = PlayerManager.Instance.Animator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(attackTime * 0.75f);
                EnemyManager.Instance.EnemyHitAnimation();
            }
            else if (selectedCard.C_Type == "Support")
            {
                BattleSystem.ExecuteDefence(PlayerManager.Instance.Player, EnemyManager.Instance.Enemy, selectedCard, value);
                PlayerManager.Instance.PlayAttackAnimation();
            }
            else if (selectedCard.C_Type == "Special")
            {
                BattleSystem.ExecuteSpecial(PlayerManager.Instance.Player, EnemyManager.Instance.Enemy, selectedCard, value);
                PlayerManager.Instance.PlayAttackAnimation();
            }

        }
        else
        {
            // 적 공격 → 플레이어 피격
            if (selectedCard.C_Type == "Action")
            {
                BattleSystem.ExecuteAttack(EnemyManager.Instance.Enemy, PlayerManager.Instance.Player, selectedCard, value);
                EnemyManager.Instance.EnemyAttackAnimation();
                float attackTime = EnemyManager.Instance.Animator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(attackTime * 0.75f);
                PlayerManager.Instance.PlayHitAnimation();
            }
            else if (selectedCard.C_Type == "Support")
            {
                BattleSystem.ExecuteDefence(EnemyManager.Instance.Enemy, PlayerManager.Instance.Player, selectedCard, value);
                EnemyManager.Instance.EnemyAttackAnimation();
            }
            else if (selectedCard.C_Type == "Special")
            {
                BattleSystem.ExecuteSpecial(EnemyManager.Instance.Enemy, PlayerManager.Instance.Player, selectedCard, value);
                EnemyManager.Instance.EnemyAttackAnimation();
            }

            
        }

        StartCoroutine(SwitchTurnWithDelay(switchTurnDelay));
    }

    private IEnumerator SwitchTurnWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (PlayerManager.Instance.Player.GetCurrentHp() <= 0 ||
            EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
        {
            yield break; // 사망 시 턴 전환 안 함
        }

        GameManager.Instance.SwitchTurn();
    }
}
