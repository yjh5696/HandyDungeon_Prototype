using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Slider = UnityEngine.UI.Slider;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text hpText;
    private Character _character;

    public void SetCharacter(Character character)
    {
        _character = character;
        if (_character)
            HpChanged(_character.GetCurrentHp(), _character.GetMaxHp());
    }

    public void HpChanged(float currentHp, float maxHp)
    {
        if (currentHp < 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
        }
        
        hpText.text = $"{currentHp:F2} / {maxHp:F2}";
        
        slider.maxValue = maxHp;
        slider.value = currentHp;
    }
}
