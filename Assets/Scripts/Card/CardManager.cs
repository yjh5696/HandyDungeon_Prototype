using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    private List<Card> _cards;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _cards = new List<Card>();
    }
}
