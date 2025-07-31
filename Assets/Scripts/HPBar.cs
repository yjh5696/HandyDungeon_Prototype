using System;
using TMPro;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Sprite[] hpSprites;
    [SerializeField] private SpriteRenderer hpRenderers;
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
        float ratio = currentHp / maxHp;
        hpText.text = $"{currentHp:F2} / {maxHp:F2}";
        if (ratio is 1)
        {
            hpRenderers.sprite = hpSprites[0];
        }
        else if (ratio is < 1 and > 4f / 6f)
        {
            hpRenderers.sprite = hpSprites[1];
        }
        else if (ratio is <= 4f / 6f and > 3f / 6f)
        {
            hpRenderers.sprite = hpSprites[2];
        }
        else if (ratio is <= 3f / 6f and > 2f / 6f)
        {
            hpRenderers.sprite = hpSprites[3];
        }
        else if (ratio is <= 2f / 6f and > 1f / 6f)
        {
            hpRenderers.sprite = hpSprites[4];
        }
        else if (ratio is <= 1f / 6f and > 0)
        {
            hpRenderers.sprite = hpSprites[5];
        }
        else
        {
            hpRenderers.sprite = null;
        }
    }
}
