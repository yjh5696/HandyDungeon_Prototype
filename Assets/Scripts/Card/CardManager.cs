using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    public CardSO selectedCard; //3가지 액션카드 중 현재 선택된 카드
    
    [SerializeField] private List<CardSO> cards;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text specialText;
    [SerializeField] private Card card;
    private CardSO _currentAttackCard;
    private CardSO _currentDefenseCard;
    private CardSO _currentSpecialCard;
    private Style _currentStyle; //3가지 버튼 중 하나 누르면 선택됨

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void DrawCard() //카드 랜덤 뽑기
    {
        int result = Random.Range(0, cards.Count);
        _currentAttackCard = cards[result];
        attackText.text = _currentAttackCard.CardName;
    }

    public void SetStyleToAttack()
    {
        _currentStyle = Style.Attack;
        card.SetCard(_currentAttackCard);
    }

    public void UseCard()
    {
        switch (_currentStyle)
        {
            case Style.Attack:
                selectedCard = _currentAttackCard;
                break;
            case Style.Defence:
                break;
            case Style.Special:
                break;
            default:
                break;
        }
        
        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog($"플레이어가 {selectedCard.CardName}을 사용하였습니다!");
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog("주사위를 터치하여 굴려주세요.");
    }
    
}
