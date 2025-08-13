using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] Choice choice1;
    [SerializeField] Choice choice2;
    [SerializeField] Choice choice3;
    [SerializeField] Choice choice4;
    private List<Choice> _choiceList;
    private Choice _currentChoice;
    public static ChoiceManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _choiceList = new List<Choice>();
        _choiceList.Add(choice1);
        _choiceList.Add(choice2);
        _choiceList.Add(choice3);
        _choiceList.Add(choice4);
    }

    public void GetRandomChoice()
    {
        
        
    }
}
