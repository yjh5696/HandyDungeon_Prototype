using System;
using TMPro;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Sprite[] hpSprites;
    [SerializeField] private SpriteRenderer hpRenderers;

    private void Start()
    {
        HpChanged(GetComponentInParent<Character>().GetCurrentHp(), GetComponentInParent<Character>().GetMaxHp()); //생성 시 실행
    }

    public void HpChanged(float currentHp, float maxHp)
    { 
        float ratio = currentHp / maxHp;
        if (ratio is <= 1 and > 5f / 6f)
        {
            hpRenderers.sprite = hpSprites[0];
        }
        else if (ratio is <= 5f / 6f and > 4f / 6f)
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
