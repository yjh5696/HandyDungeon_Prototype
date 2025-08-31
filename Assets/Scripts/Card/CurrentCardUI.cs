using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentCardUI : MonoBehaviour
{
    [Header("Icons")]
    public Image baseElementIcon;     // BaseElement 오브젝트
    public Image baseElement2Icon;    // BaseElement2 오브젝트
    public Image plusElementIcon;     // PlusElement 오브젝트
    public Image upgradeElementIcon;  // UpgradeElement 오브젝트

    [Header("Sprites")]
    public Sprite fireIcon;
    public Sprite waterIcon;
    public Sprite airIcon;
    public Sprite landIcon;
    public Sprite ignitionIcon;
    public Sprite fervorIcon;
    public Sprite galeIcon;
    public Sprite guardIcon;
    public Sprite recoveryIcon;
    public Sprite vibrationIcon;
    public Sprite burndownIcon;
    public Sprite iceIcon;
    public Sprite rockIcon;
    public Sprite explosionIcon;

    [Header("Texts")]
    public TMP_Text elementText;           // ElementText 오브젝트
    public TMP_Text upgradeElementText;    // UpgradeElementText 오브젝트
    public TMP_Text upgradeCountText;      // Upgrade_Count 오브젝트

    [Header("Etc")]
    public GameObject effectCardPanel;     // EffectCard 루트 오브젝트
    public Button closeButton;             // Close 버튼

    public Sprite GetIconSprite(string state)
    {
        switch (state)
        {
            case "Fire": return fireIcon;
            case "Water": return waterIcon;
            case "Air": return airIcon;
            case "Land": return landIcon;
            case "Ignition": return ignitionIcon;
            case "Fervor": return fervorIcon;
            case "Gale": return galeIcon;
            case "Guard": return guardIcon;
            case "Recovery": return recoveryIcon;
            case "Vibration": return vibrationIcon;
            case "Burndown": return burndownIcon;
            case "Ice": return iceIcon;
            case "Rock": return rockIcon;
            case "Explosion": return explosionIcon;
            case "None": return null;
            default: return null;
        }
    }

    public Sprite GetPlusIconSprite(string state)
    {
        switch (state)
        {
            case "Fire": return  airIcon; 
            case "Water": return landIcon;
            case "Air": return waterIcon;
            case "Land": return fireIcon;
            case "Fervor": return recoveryIcon;
            case "Gale": return guardIcon;
            case "Guard": return galeIcon;
            case "Recovery": return fervorIcon;
            case "None": return null;
            default: return null;
        }
    }
    public Sprite GetUpgradeIconSprite(string state)
    {
        switch (state)
        {
            case "Fire": return ignitionIcon;
            case "Water": return iceIcon;
            case "Air": return rockIcon;
            case "Land": return explosionIcon;
            case "Fervor": return burndownIcon;
            case "Gale": return vibrationIcon;
            case "Guard": return vibrationIcon;
            case "Recovery": return burndownIcon;
            case "None": return null;
            default: return null;
        }
    }

    public string GetUpgradeElement(string state)
    {
        switch (state)
        {
            case "Fire": return "Ignition";
            case "Water": return "Rock";
            case "Air": return "Ice";
            case "Land": return "Explosion";
            case "Fervor": return "Burndown";
            case "Gale": return "Vibration";
            case "Guard": return "Vibration";
            case "Recovery": return "Burndown";
            case "None": return null;
            default: return null;
        }
    }

    public void OpenCurrentCard()
    {
        CardDataSO card = CardManager.Instance.selectedCard;

        if (card == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // 카드 속성 아이콘 및 설명 출력
        if(card.C_Type == "Action")
        {
            baseElementIcon.sprite = GetIconSprite(card.Debuff_Type);
            elementText.text = GetElementDescription(card.Debuff_Type);
            baseElement2Icon.sprite = GetIconSprite(card.Debuff_Type);
            plusElementIcon.sprite = GetPlusIconSprite(card.Debuff_Type);
            upgradeElementIcon.sprite = GetUpgradeIconSprite(card.Debuff_Type);
            upgradeElementText.text = GetElementDescription(GetUpgradeElement(card.Debuff_Type));
        }
        else
        {
            baseElementIcon.sprite = GetIconSprite(card.Buff_Type);
            elementText.text = GetElementDescription(card.Buff_Type);
            baseElement2Icon.sprite = GetIconSprite(card.Buff_Type);
            plusElementIcon.sprite = GetPlusIconSprite(card.Buff_Type);
            upgradeElementIcon.sprite = GetUpgradeIconSprite(card.Buff_Type);
            upgradeElementText.text = GetElementDescription(GetUpgradeElement(card.Buff_Type));
        }

        string remainCountText = CardManager.Instance.GetEnhanceCountString(card.C_Id);
        upgradeCountText.text = $"{remainCountText}";

        gameObject.SetActive(true);
    }

    public void Close()
    {
        effectCardPanel.SetActive(false);
    }

    // 아래 메서드는 실제 프로젝트 구조 맞게 구현 필요
    string GetElementDescription(string type)
    {
        return type switch
        {
            "Fire" => "점화: 적의 턴이 종료될 때, 피해를 입힙니다. 피해를 준 후 스택이 1 감소합니다.",
            "Water" => "젖음: 적의 행동 시 카드 효과가 감소합니다. 그 후 플레이어는 체력을 회복합니다. 턴 종료 후 스택이 1 감소합니다. ",
            "Air" => "교란: 적 주사위의 눈이 교란의 스택 수와 같은 짝수 또는 홀수일 경우, 교란 스택만큼 카드 효과가 감소합니다. 턴 종료 후 스택이 1 감소합니다.",
            "Land" => "균열: 적이 받는 피해를 증가시킵니다. 턴 종료 후 스택이 1 감소합니다.",
            "Ignition" => "연소: 매턴 강한 지속 피해를 준다. 적의 턴이 끝날 시 1스택 감소",
            "Fervor" => "열정: 주사위 결과에 추가 피해를 부여합니다. 턴 종료 후 스택이 1 감소합니다.",
            "Gale" => "순풍: 주사위의 눈이 순풍 스택 수와 같은 짝수 또는 홀수일 경우 순풍의 스택 수만큼 추가 효과를 얻습니다. 턴 종료 후 스택이 1 감소합니다.",
            "Guard" => "수호: 받는 피해를 만큼 감소시킵니다. 턴 종료 후 스택이 1 감소합니다.",
            "Recovery" => "회복: 재생 스택 수만큼 주사위 결과에 추가 보호막 또는 회복을 부여합니다. 턴 종료 후 스택이 1 감소합니다",
            "Vibration" => "진동: 적에게 공격을 받은 후, 원래 피해량 만큼 데미지를 되돌려 줍니다. 효과 발동 후 1스택이 감소됩니다.",
            "Burndown" => "소화: 적에게 피해를 줍니다. 또한 액션 카드 사용시 사라지는 데미지 감소를 얻습니다.",
            "Ice" => "동상: 강한 피해를 줍니다. 주사위의 눈과 동상 스택의 홀짝이 일치할 경우, 다음에 상대가 사용하는 카드의 효과를 감소시킵니다.",
            "Rock" => "암반화: 강한 피해를 주고, 2 × 젖음의 스택 수만큼 플레이어는 보호막을 획득합니다.",
            "Explosion" => "분화: 적은 추가로 피해를 받으며, 강한 피해를 입히며 이때 추가 피해를 같이 적용됩니다.",
            _ => "속성이 없습니다."
        };
    }

}

